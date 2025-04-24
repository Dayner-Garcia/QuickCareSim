using QuickCareSim.Application.Interfaces.Repositories;
using QuickCareSim.Application.Interfaces.Services.Core;
using QuickCareSim.Domain.Entities;

namespace QuickCareSim.Application.Services.Core
{
    public class SimulationMetricsService : ISimulationMetricsService
    {
        private readonly IGenericRepository<UrgencyWaitMetric> _urgencyMetricRepo;
        private readonly IGenericRepository<PerformanceMetric> _performanceMetricRepo;
        private readonly IGenericRepository<SimulationRun> _simulationRepo;

        public SimulationMetricsService(
            IGenericRepository<UrgencyWaitMetric> urgencyMetricRepo,
            IGenericRepository<PerformanceMetric> performanceMetricRepo,
            IGenericRepository<SimulationRun> simulationRepo)
        {
            _urgencyMetricRepo = urgencyMetricRepo;
            _performanceMetricRepo = performanceMetricRepo;
            _simulationRepo = simulationRepo;
        }

        public async Task StoreUrgencyMetricsAsync(IEnumerable<Patient> patients, int runId)
        {
            var existentes = await _urgencyMetricRepo.GetAllAsync();
            var aEliminar = existentes
                .Where(m => m.SimulationRunId == runId)
                .Select(m => m.Id)
                .ToList();

            foreach (var id in aEliminar)
            {
                await _urgencyMetricRepo.DeleteAsync(id);
            }

            var nuevos = patients
                .Where(p => p.AttendedTime.HasValue)
                .GroupBy(p => p.Urgency)
                .Select(g => new UrgencyWaitMetric
                {
                    UrgencyLevel = g.Key,
                    AverageWaitSeconds = g.Average(p => (p.AttendedTime!.Value - p.ArrivalTime).TotalSeconds),
                    TotalPatients = g.Count(),
                    SimulationRunId = runId
                });

            foreach (var metric in nuevos)
            {
                await _urgencyMetricRepo.AddAsync(metric);
            }
        }

        public async Task StorePerformanceMetricsAsync(IEnumerable<AttentionLog> logs, int runId)
        {
            var grouped = logs
                .Where(l => l.SimulationRunId == runId)
                .GroupBy(l => l.DoctorId);

            foreach (var group in grouped)
            {
                var metric = new PerformanceMetric
                {
                    DoctorId = group.Key,
                    PatientsAttended = group.Count(),
                    AverageAttentionTimeSeconds = group.Average(l => (l.EndTime - l.StartTime).TotalSeconds),
                    SimulationRunId = runId
                };

                await _performanceMetricRepo.AddAsync(metric);
            }
        }

        public async Task UpdateSpeedupAndEfficiencyAsync(SimulationRun run, double tiempoSecuencial)
        {
            if (run.ProcessorsUsed <= 1 || run.ExecutionTimeSeconds <= 0 || tiempoSecuencial <= 0)
            {
                run.Speedup = -1;
                run.Efficiency = -1;
            }
            else
            {
                double speedup = tiempoSecuencial / run.ExecutionTimeSeconds;
                double efficiency = speedup / run.ProcessorsUsed;

                run.Speedup = Math.Round(speedup, 2);
                run.Efficiency = Math.Round(efficiency, 2);
            }

            await _simulationRepo.UpdateAsync(run);
        }
    }
}