using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuickCareSim.Application.Interfaces.Repositories;
using QuickCareSim.Application.Interfaces.Services.Core;
using QuickCareSim.Application.Interfaces.Services.Executors;
using QuickCareSim.Application.Interfaces.Services.Strategies;
using QuickCareSim.Application.ViewModels.UrgencyRoom;
using QuickCareSim.Domain.Entities;
using QuickCareSim.Domain.Enums;

namespace QuickCareSim.Application.Services.Executors;

public class ParallelSimulationExcecutor : IParallelSimulationExecutor
{
    private readonly IGenericRepository<SimulationRun> _simulationRunRepository;
    private readonly IGenericRepository<Doctor> _doctorRepository;
    private readonly IPatientService _patientService;
    private readonly IAttentionStrategyFactoryService _attentionStrategyFactory;
    private readonly ISimulationMetricsService _metricsService;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ParallelSimulationExcecutor(
        IGenericRepository<SimulationRun> simulationRunRepository,
        IGenericRepository<Doctor> doctorRepository, IPatientService patientService,
        IAttentionStrategyFactoryService attentionStrategyFactory, ISimulationMetricsService metricsService,
        IServiceScopeFactory serviceScopeFactory)
    {
        _simulationRunRepository = simulationRunRepository;
        _doctorRepository = doctorRepository;
        _patientService = patientService;
        _attentionStrategyFactory = attentionStrategyFactory;
        _metricsService = metricsService;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<int> ExecuteNewAsync(SimulationParametersViewModel parameters, CancellationToken token)
    {
        var availableDoctors =
            (await _doctorRepository.GetAllAsync(q =>
                q.Where(d => d.Status == DoctorStatus.AVAILABLE).AsNoTracking()))
            .Take(parameters.DoctorsToUse).ToList();

        if (!availableDoctors.Any())
            throw new Exception("No hay doctores disponibles");

        var run = new SimulationRun
        {
            RunAt = DateTime.Now,
            StrategyUsed = parameters.Strategy,
            TotalPatients = parameters.TotalPatients,
            TotalDoctors = availableDoctors.Count,
            ProcessorsUsed = availableDoctors.Count
        };

        await _simulationRunRepository.AddAsync(run);
        await ExecuteSimulationInternal(run.Id, availableDoctors, parameters, token);
        return run.Id;
    }

    public async Task ExecuteSimulationInternal(int simulationId, List<Doctor> availableDoctors,
        SimulationParametersViewModel parameters, CancellationToken token)
    {
        var (queue, patientDict) = _patientService.GeneratePatients(parameters.TotalPatients);
        var strategy = _attentionStrategyFactory.GetStrategy(parameters.Strategy);
        var attentionLogs = new ConcurrentBag<AttentionLog>();
        var stopwatch = Stopwatch.StartNew();

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = availableDoctors.Count,
            CancellationToken = token
        };

        await Parallel.ForEachAsync(availableDoctors, options, async (doctor, ct) =>
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var doctorRepo = scope.ServiceProvider.GetRequiredService<IGenericRepository<Doctor>>();

            doctor.Status = DoctorStatus.BUSY;
            await doctorRepo.UpdateAsync(doctor);

            await strategy.ExecuteAsync(queue, patientDict, 0, simulationId, (patient) =>
            {
                patient.DoctorId = doctor.UserId;
                attentionLogs.Add(new AttentionLog
                {
                    DoctorId = doctor.UserId,
                    PatientId = patient.UserId,
                    StartTime = patient.AttendedTime!.Value,
                    EndTime = DateTime.UtcNow,
                    StrategyUsed = parameters.Strategy.ToString(),
                    SimulationRunId = simulationId
                });
                return Task.CompletedTask;
            }, ct);

            doctor.Status = DoctorStatus.AVAILABLE;
            await doctorRepo.UpdateAsync(doctor);
        });

        stopwatch.Stop();
        var run = await _simulationRunRepository.GetByIdAsync(simulationId);

        if (run != null)
        {
            run.ExecutionTimeSeconds = stopwatch.Elapsed.TotalSeconds;
            run.TotalPatientsAttended = attentionLogs.Count;
            await _simulationRunRepository.UpdateAsync(run);
        }

        await Parallel.ForEachAsync(attentionLogs, new ParallelOptions { MaxDegreeOfParallelism = 8 },
            async (log, ct) =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var logRepo = scope.ServiceProvider.GetRequiredService<IGenericRepository<AttentionLog>>();
                await logRepo.AddAsync(log);
            });

        await _metricsService.StoreUrgencyMetricsAsync(patientDict.Values, simulationId);
        await _metricsService.StorePerformanceMetricsAsync(attentionLogs, simulationId);
    }

    public async Task ExecuteReusingIdAsync(int simulationId, SimulationParametersViewModel parameters,
        CancellationToken token)
    {
        var availableDoctors = (await _doctorRepository.GetAllAsync(q =>
                q.Where(d => d.Status == DoctorStatus.AVAILABLE).AsNoTracking()))
            .Take(parameters.DoctorsToUse).ToList();

        if (!availableDoctors.Any())
            throw new Exception("No hay doctores disponibles.");

        await ExecuteSimulationInternal(simulationId, availableDoctors, parameters, token);
    }
}