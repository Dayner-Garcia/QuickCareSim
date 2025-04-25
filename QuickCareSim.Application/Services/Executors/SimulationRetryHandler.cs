using QuickCareSim.Application.Interfaces.Repositories;
using QuickCareSim.Application.Interfaces.Services.Executors;
using QuickCareSim.Application.ViewModels.UrgencyRoom;
using QuickCareSim.Domain.Entities;

namespace QuickCareSim.Application.Services.Executors;

public class SimulationRetryHandler : ISimulationRetryHandler
{
    
    private readonly IGenericRepository<SimulationRun> _simulationRunRepository;
    private readonly IParallelSimulationExecutor _parallelSimulationExecutor;
    private readonly ISequentialSimulationExecutor _sequentialSimulationExecutor;

    public SimulationRetryHandler(IGenericRepository<SimulationRun> simulationRunRepository, IParallelSimulationExecutor parallelSimulationExecutor, ISequentialSimulationExecutor sequentialSimulationExecutor)
    {
        _simulationRunRepository = simulationRunRepository;
        _parallelSimulationExecutor = parallelSimulationExecutor;
        _sequentialSimulationExecutor = sequentialSimulationExecutor;
    }
    
    
    public async Task RetryAsync(int simulationId, CancellationToken token)
    {
        var run = await _simulationRunRepository.GetByIdAsync(simulationId)
            ?? throw new Exception("Simulacion no encontrada.");

        var parameters = new SimulationParametersViewModel
        {
            TotalPatients = run.TotalPatients,
            Strategy = run.StrategyUsed,
            ExecutionMode = run.ProcessorsUsed == 1 ? "Sequential" : "Parallel",
            DoctorsToUse = run.TotalDoctors
        };

        run.RealExecutionTimeSeconds = 0;
        run.RealExecutionTimeSeconds = 0;
        run.TotalPatientsAttended = 0;
        run.MetricsFinalized = false;
        
        await _simulationRunRepository.UpdateAsync(run);
        
        if(parameters.ExecutionMode == "Sequential")
            await _sequentialSimulationExecutor.ExecuteReusingIdAsync(simulationId,parameters, token);
        else
            await _parallelSimulationExecutor.ExecuteReusingIdAsync(simulationId, parameters, token);
    }
}