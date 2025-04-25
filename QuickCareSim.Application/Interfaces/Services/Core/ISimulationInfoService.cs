using QuickCareSim.Application.ViewModels.UrgencyRoom;

namespace QuickCareSim.Application.Interfaces.Services.Core
{
    public interface ISimulationInfoService
    {
        Task<SimulationResultViewModel?> GetSimulationResultAsync(int id);
        Task<List<UrgencyWaitMetricViewModel>> GetUrgencyMetricsAsync(int simulationId);
        Task<List<PerformanceMetricViewModel>> GetPerformanceMetricsAsync(int simulationId);
        Task<List<SimulationResultViewModel>> GetAllSimulationsAsync();
    }
}
