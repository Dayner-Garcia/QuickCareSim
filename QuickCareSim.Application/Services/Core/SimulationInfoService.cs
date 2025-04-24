using QuickCareSim.Application.Interfaces.Repositories;
using QuickCareSim.Application.Interfaces.Services.Core;
using QuickCareSim.Application.ViewModels.UrgencyRoom;
using QuickCareSim.Domain.Entities;

namespace QuickCareSim.Application.Services.Core
{
    public class SimulationInfoService : ISimulationInfoService
    {
        private readonly IGenericRepository<SimulationRun> _simulationRepo;
        private readonly IGenericRepository<UrgencyWaitMetric> _urgencyMetricRepo;
        private readonly IGenericRepository<PerformanceMetric> _performanceMetricRepo;

        public SimulationInfoService(
            IGenericRepository<SimulationRun> simulationRepo,
            IGenericRepository<UrgencyWaitMetric> urgencyMetricRepo,
            IGenericRepository<PerformanceMetric> performanceMetricRepo)
        {
            _simulationRepo = simulationRepo;
            _urgencyMetricRepo = urgencyMetricRepo;
            _performanceMetricRepo = performanceMetricRepo;
        }

        public async Task<SimulationResultViewModel?> GetSimulationResultAsync(int id)
        {
            var run = await _simulationRepo.GetByIdAsync(id);
            if (run == null) return null;

            return new SimulationResultViewModel
            {
                Id = run.Id,
                RunAt = run.RunAt,
                StrategyUsed = run.StrategyUsed.ToString(),
                TotalDoctors = run.TotalDoctors,
                TotalPatients = run.TotalPatients,
                TotalPatientsAttended = run.TotalPatientsAttended,
                ExecutionTimeSeconds = run.ExecutionTimeSeconds,
                RealExecutionTimeSeconds = run.RealExecutionTimeSeconds,
                PatientsPerMinute = run.PatientsPerMinute,
                ProcessorsUsed = run.ProcessorsUsed,
                Speedup = run.Speedup,
                Efficiency = run.Efficiency,
                ExecutionMode = run.ProcessorsUsed == 1 ? "Sequential" : "Parallel"
            };
        }

        public async Task<List<UrgencyWaitMetricViewModel>> GetUrgencyMetricsAsync(int simulationId)
        {
            var metrics = await _urgencyMetricRepo.GetAllAsync();
            return metrics
                .Where(m => m.SimulationRunId == simulationId)
                .Select(m => new UrgencyWaitMetricViewModel
                {
                    UrgencyLevel = m.UrgencyLevel.ToString(),
                    AverageWaitSeconds = Math.Round(m.AverageWaitSeconds, 2),
                    TotalPatients = m.TotalPatients
                })
                .ToList();
        }

        public async Task<List<PerformanceMetricViewModel>> GetPerformanceMetricsAsync(int simulationId)
        {
            var data = await _performanceMetricRepo.GetAllAsync();
            return data
                .Where(m => m.SimulationRunId == simulationId)
                .Select(m => new PerformanceMetricViewModel
                {
                    DoctorId = m.DoctorId,
                    PatientsAttended = m.PatientsAttended,
                    AverageAttentionTimeSeconds = Math.Round(m.AverageAttentionTimeSeconds, 2)
                })
                .ToList();
        }

        public async Task<List<SimulationResultViewModel>> GetAllSimulationsAsync()
        {
            var runs = await _simulationRepo.GetAllAsync();
            return runs.Select(run => new SimulationResultViewModel
            {
                Id = run.Id,
                RunAt = run.RunAt,
                StrategyUsed = run.StrategyUsed.ToString(),
                TotalDoctors = run.TotalDoctors,
                TotalPatients = run.TotalPatients,
                TotalPatientsAttended = run.TotalPatientsAttended,
                ExecutionTimeSeconds = run.ExecutionTimeSeconds,
                RealExecutionTimeSeconds = run.RealExecutionTimeSeconds,
                PatientsPerMinute = run.PatientsPerMinute,
                ProcessorsUsed = run.ProcessorsUsed,
                Speedup = run.Speedup,
                Efficiency = run.Efficiency,
                ExecutionMode = run.ProcessorsUsed == 1 ? "Sequential" : "Parallel"
            }).OrderByDescending(r => r.RunAt).ToList();
        }
    }
}