namespace QuickCareSim.Application.Interfaces.Services.Executors;

public interface ISimulationRetryHandler
{
    Task RetryAsync(int simulationId, CancellationToken token);
}