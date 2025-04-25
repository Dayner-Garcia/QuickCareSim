using QuickCareSim.Application.ViewModels.UrgencyRoom;
using QuickCareSim.Domain.Entities;

namespace QuickCareSim.Application.Interfaces.Services.Executors;

public interface IParallelSimulationExecutor
{
    Task<int> ExecuteNewAsync(SimulationParametersViewModel parameters, CancellationToken token);
    Task ExecuteReusingIdAsync(int simulationId, SimulationParametersViewModel parameters, CancellationToken token);
    Task ExecuteSimulationInternal(int simulationId, List<Doctor> availableDoctors,
        SimulationParametersViewModel parameters, CancellationToken token);
}