using Microsoft.Extensions.DependencyInjection;
using QuickCareSim.Application.Interfaces.Repositories;
using QuickCareSim.Application.Interfaces.Services.Core;
using QuickCareSim.Application.Interfaces.Services.Executors;
using QuickCareSim.Application.ViewModels.UrgencyRoom;
using QuickCareSim.Domain.Entities;
using System.Diagnostics;

namespace QuickCareSim.Application.Services.Executors
{
    public class SpeedupCalculator : ISpeedupCalculator
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public SpeedupCalculator(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task CalculateAsync(SimulationRun parallelRun, SimulationParametersViewModel parameters,
            CancellationToken token)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var scopedExecutor = scope.ServiceProvider.GetRequiredService<ISequentialSimulationExecutor>();
                var scopedMetrics = scope.ServiceProvider.GetRequiredService<ISimulationMetricsService>();
                var scopedRepo = scope.ServiceProvider.GetRequiredService<IGenericRepository<SimulationRun>>();

                int sequentialId = await scopedExecutor.ExecuteNewAsync(parameters, token);
                var sequentialRun = await scopedRepo.GetByIdAsync(sequentialId);

                if (sequentialRun == null || sequentialRun.ExecutionTimeSeconds <= 0) throw new Exception("La simulacion secuencial fallo.");

                double tiempoSecuencial = sequentialRun.ExecutionTimeSeconds;
                await scopedMetrics.UpdateSpeedupAndEfficiencyAsync(parallelRun, tiempoSecuencial);

                parallelRun.MetricsFinalized = true;
                await scopedRepo.UpdateAsync(parallelRun);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"errrror {ex.Message}");

                using var errorScope = _scopeFactory.CreateScope();
                var scopedRepo = errorScope.ServiceProvider.GetRequiredService<IGenericRepository<SimulationRun>>();

                if (parallelRun.Speedup == null && parallelRun.Efficiency == null)
                {
                    parallelRun.MetricsFinalized = true;
                    parallelRun.Speedup = -1;
                    parallelRun.Efficiency = -1;
                    await scopedRepo.UpdateAsync(parallelRun);
                }
            }
        }
    }
}