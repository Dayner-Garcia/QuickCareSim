using QuickCareSim.Application.ViewModels.UrgencyRoom;
using QuickCareSim.Domain.Entities;

namespace QuickCareSim.Application.Interfaces.Services.Executors
{
    public interface ISpeedupCalculator
    {
        Task CalculateAsync(SimulationRun parallelRun, SimulationParametersViewModel parameters, CancellationToken token);
    }
}
