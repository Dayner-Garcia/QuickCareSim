using QuickCareSim.Application.ViewModels.UrgencyRoom;

namespace QuickCareSim.Application.Interfaces.Services.Executors
{
    public interface ISequentialSimulationExecutor
    {
        Task<int> ExecuteNewAsync(SimulationParametersViewModel parameters, CancellationToken token);
        Task ExecuteReusingIdAsync(int simulationId, SimulationParametersViewModel parameters, CancellationToken token);
    }
}