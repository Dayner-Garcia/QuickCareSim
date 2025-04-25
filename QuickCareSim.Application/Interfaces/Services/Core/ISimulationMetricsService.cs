using QuickCareSim.Domain.Entities;

namespace QuickCareSim.Application.Interfaces.Services.Core
{
    public interface ISimulationMetricsService
    {
        Task StoreUrgencyMetricsAsync(IEnumerable<Patient> patients, int runId);
        Task StorePerformanceMetricsAsync(IEnumerable<AttentionLog> logs, int runId);
        Task UpdateSpeedupAndEfficiencyAsync(SimulationRun run, double tiempoSecuencial);
    }
}