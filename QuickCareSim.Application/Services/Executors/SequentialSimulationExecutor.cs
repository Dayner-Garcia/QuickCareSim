using Microsoft.Extensions.DependencyInjection;
using QuickCareSim.Application.Interfaces.Repositories;
using QuickCareSim.Application.Interfaces.Services;
using QuickCareSim.Application.Interfaces.Services.Core;
using QuickCareSim.Application.Interfaces.Services.Executors;
using QuickCareSim.Application.Interfaces.Services.Strategies;
using QuickCareSim.Application.Utils;
using QuickCareSim.Application.ViewModels.UrgencyRoom;
using QuickCareSim.Domain.Entities;
using QuickCareSim.Domain.Enums;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace QuickCareSim.Application.Services.Executors
{
    public class SequentialSimulationExecutor : ISequentialSimulationExecutor
    {
        private readonly IGenericRepository<SimulationRun> _simulationRepo;
        private readonly IGenericRepository<Doctor> _doctorRepository;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IAttentionStrategyFactoryService _strategyFactory;
        private readonly ISimulationMetricsService _metricsService;

        public SequentialSimulationExecutor(
            IGenericRepository<SimulationRun> simulationRepo,
            IGenericRepository<Doctor> doctorRepository,
            IServiceScopeFactory scopeFactory,
            IAttentionStrategyFactoryService strategyFactory,
            ISimulationMetricsService metricsService)
        {
            _simulationRepo = simulationRepo;
            _doctorRepository = doctorRepository;
            _scopeFactory = scopeFactory;
            _strategyFactory = strategyFactory;
            _metricsService = metricsService;
        }

        public async Task<int> ExecuteNewAsync(SimulationParametersViewModel parameters, CancellationToken token)
        {
            var availableDoctors = (await _doctorRepository.GetAllAsync())
                .Where(d => d.Status == DoctorStatus.AVAILABLE)
                .Take(1).ToList();

            if (!availableDoctors.Any())
                throw new Exception("No hay doctores disponibles.");

            var run = new SimulationRun
            {
                RunAt = DateTime.UtcNow,
                StrategyUsed = parameters.Strategy,
                TotalPatients = parameters.TotalPatients,
                TotalDoctors = 1,
                ProcessorsUsed = 1
            };

            await _simulationRepo.AddAsync(run);
            await ExecuteSimulationInternal(run.Id, availableDoctors.First(), parameters, token);
            return run.Id;
        }

        public async Task ExecuteReusingIdAsync(int simulationId, SimulationParametersViewModel parameters,
            CancellationToken token)
        {
            var availableDoctor = (await _doctorRepository.GetAllAsync())
                .FirstOrDefault(d => d.Status == DoctorStatus.AVAILABLE);

            if (availableDoctor == null)
                throw new Exception("No hay doctores disponibles.");

            await ExecuteSimulationInternal(simulationId, availableDoctor, parameters, token);
        }

        private async Task ExecuteSimulationInternal(int simulationId, Doctor doctor,
            SimulationParametersViewModel parameters, CancellationToken token)
        {
            var queue = new ConcurrentQueue<string>();
            var patientDict = new ConcurrentDictionary<string, Patient>();
            var rand = new Random();

            for (int i = 0; i < parameters.TotalPatients; i++)
            {
                var urgency = (UrgencyLevel)rand.Next(1, 5);
                var patient = new Patient
                {
                    UserId = Guid.NewGuid().ToString(),
                    ArrivalTime = DateTime.UtcNow,
                    Urgency = urgency,
                    Status = PatientStatus.WAITING
                };
                queue.Enqueue(patient.UserId);
                patientDict.TryAdd(patient.UserId, patient);
            }

            var strategy = _strategyFactory.GetStrategy(parameters.Strategy);
            var stopwatch = Stopwatch.StartNew();

            while (!token.IsCancellationRequested && !queue.IsEmpty)
            {
                if (!queue.TryDequeue(out var id)) continue;
                if (!patientDict.TryGetValue(id, out var patient)) continue;

                patient.Status = PatientStatus.IN_ATTENTION;
                patient.AttendedTime = DateTime.UtcNow;

                await Task.Delay(UrgencyUtils.GetSimulatedAttentionTime(patient.Urgency) * 1000, token);

                patient.Status = PatientStatus.ATTENDED;
                patient.DoctorId = doctor.UserId;

                using var scope = _scopeFactory.CreateScope();
                var logRepo = scope.ServiceProvider.GetRequiredService<IGenericRepository<AttentionLog>>();
                await logRepo.AddAsync(new AttentionLog
                {
                    DoctorId = doctor.UserId,
                    PatientId = patient.UserId,
                    StartTime = patient.AttendedTime.Value,
                    EndTime = DateTime.UtcNow,
                    StrategyUsed = parameters.Strategy.ToString(),
                    SimulationRunId = simulationId
                });
            }

            stopwatch.Stop();
            var run = await _simulationRepo.GetByIdAsync(simulationId);

            if (run != null)
            {
                run.ExecutionTimeSeconds = stopwatch.Elapsed.TotalSeconds;
                run.RealExecutionTimeSeconds = stopwatch.Elapsed.TotalSeconds;
                run.TotalPatientsAttended = parameters.TotalPatients;
                await _simulationRepo.UpdateAsync(run);
            }

            await _metricsService.StoreUrgencyMetricsAsync(patientDict.Values, simulationId);

            using (var scope = _scopeFactory.CreateScope())
            {
                var logRepo = scope.ServiceProvider.GetRequiredService<IGenericRepository<AttentionLog>>();
                var logs = await logRepo.GetAllAsync();
                await _metricsService.StorePerformanceMetricsAsync(logs.Where(l => l.SimulationRunId == simulationId),
                    simulationId);
            }
        }
    }
}