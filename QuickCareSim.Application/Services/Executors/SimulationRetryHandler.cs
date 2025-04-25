using Microsoft.Extensions.DependencyInjection;
using QuickCareSim.Application.Interfaces.Repositories;
using QuickCareSim.Application.Interfaces.Services.Executors;
using QuickCareSim.Application.ViewModels.UrgencyRoom;
using QuickCareSim.Domain.Entities;

namespace QuickCareSim.Application.Services.Executors
{
    public class SimulationRetryHandler : ISimulationRetryHandler
    {
        private readonly IGenericRepository<SimulationRun> _simulationRepo;
        private readonly IParallelSimulationExecutor _parallelExecutor;
        private readonly ISequentialSimulationExecutor _sequentialExecutor;

        public SimulationRetryHandler(
            IGenericRepository<SimulationRun> simulationRepo,
            IParallelSimulationExecutor parallelExecutor,
            ISequentialSimulationExecutor sequentialExecutor)
        {
            _simulationRepo = simulationRepo;
            _parallelExecutor = parallelExecutor;
            _sequentialExecutor = sequentialExecutor;
        }

        public async Task RetryAsync(int simulationId, CancellationToken token)
        {
            var run = await _simulationRepo.GetByIdAsync(simulationId)
                      ?? throw new Exception("Simulacion no encontrada.");

            var parameters = new SimulationParametersViewModel
            {
                TotalPatients = run.TotalPatients,
                Strategy = run.StrategyUsed,
                ExecutionMode = run.ProcessorsUsed == 1 ? "Sequential" : "Parallel",
                DoctorsToUse = run.TotalDoctors
            };

            run.ExecutionTimeSeconds = 0;
            run.RealExecutionTimeSeconds = 0;
            run.TotalPatientsAttended = 0;
            run.MetricsFinalized = false;
            await _simulationRepo.UpdateAsync(run);

            if (parameters.ExecutionMode == "Sequential")
                await _sequentialExecutor.ExecuteReusingIdAsync(simulationId, parameters, token);
            else
                await _parallelExecutor.ExecuteReusingIdAsync(simulationId, parameters, token);
        }
    }
}