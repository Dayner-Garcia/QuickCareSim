using Microsoft.Extensions.DependencyInjection;
using Moq;
using QuickCareSim.Application.Interfaces.Repositories;
using QuickCareSim.Application.Interfaces.Services.Core;
using QuickCareSim.Application.Interfaces.Services.Executors;
using QuickCareSim.Application.Services.Executors;
using QuickCareSim.Application.ViewModels.UrgencyRoom;
using QuickCareSim.Domain.Entities;
using QuickCareSim.Domain.Enums;
using QuickCareSim.Tests.Common;
using System.Collections.Concurrent;
using QuickCareSim.Application.Interfaces.Services.Strategies;
using Xunit.Abstractions;

namespace QuickCareSim.Tests.Application
{
    public class SequentialSimulationExecutorTests : TestBase
    {
        private readonly ISequentialSimulationExecutor _executor;
        private readonly ITestOutputHelper _output;

        public SequentialSimulationExecutorTests(ITestOutputHelper output)
        {
            _output = output;

            // mmockear
            var strategyMock = new Mock<IAttentionStrategyService>();
            strategyMock.Setup(s => s.ExecuteAsync(
                    It.IsAny<ConcurrentQueue<string>>(),
                    It.IsAny<ConcurrentDictionary<string, Patient>>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Func<Patient, Task>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var strategyFactoryMock = new Mock<IAttentionStrategyFactoryService>();
            strategyFactoryMock.Setup(f => f.GetStrategy(It.IsAny<StrategyType>()))
                .Returns(strategyMock.Object);

            var metricsServiceMock = new Mock<ISimulationMetricsService>();

            Context.Doctors.Add(new Doctor
            {
                UserId = Guid.NewGuid().ToString(),
                Status = DoctorStatus.AVAILABLE
            });
            Context.SaveChanges();

            _executor = new SequentialSimulationExecutor(
                Provider.GetRequiredService<IGenericRepository<SimulationRun>>(),
                Provider.GetRequiredService<IGenericRepository<Doctor>>(),
                Provider.GetRequiredService<IServiceScopeFactory>(),
                strategyFactoryMock.Object,
                metricsServiceMock.Object
            );
        }

        [Fact]
        public async Task ExecuteNewAsync_Should_CreateSimulationRun_When_DoctorAvailable()
        {
            // Arrange
            var parameters = new SimulationParametersViewModel
            {
                TotalPatients = 5,
                Strategy = StrategyType.RoundRobin
            };

            // Act
            int simId = await _executor.ExecuteNewAsync(parameters, CancellationToken.None);

            // Assert
            var run = await Context.SimulationRuns.FindAsync(simId);
            Assert.NotNull(run);
            Assert.Equal(5, run.TotalPatients);
            Assert.Equal(1, run.TotalDoctors);
            Assert.Equal(1, run.ProcessorsUsed);

            _output.WriteLine("Test de la simulacion en secuencial funcionando correctamente.");
        }
    }
}