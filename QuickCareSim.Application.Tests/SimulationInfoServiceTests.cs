using QuickCareSim.Application.Services.Core;
using QuickCareSim.Application.ViewModels.UrgencyRoom;
using QuickCareSim.Domain.Entities;
using QuickCareSim.Domain.Enums;
using QuickCareSim.Tests.Common;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using QuickCareSim.Application.Interfaces.Repositories;

namespace QuickCareSim.Tests.Application
{
    public class SimulationInfoServiceTests : TestBase
    {
        private readonly SimulationInfoService _service;
        private readonly ITestOutputHelper _output;

        public SimulationInfoServiceTests(ITestOutputHelper output)
        {
            _output = output;
            _service = new SimulationInfoService(
                Provider.GetRequiredService<IGenericRepository<SimulationRun>>(),
                Provider.GetRequiredService<IGenericRepository<UrgencyWaitMetric>>(),
                Provider.GetRequiredService<IGenericRepository<PerformanceMetric>>()
            );
        }

        [Fact]
        public async Task GetSimulationResultAsync_Should_ReturnSimulationViewModel()
        {
            // Arrange
            var run = new SimulationRun
            {
                StrategyUsed = StrategyType.RoundRobin,
                TotalPatients = 10,
                TotalDoctors = 2,
                ProcessorsUsed = 2,
                RunAt = DateTime.UtcNow,
                ExecutionTimeSeconds = 20,
                TotalPatientsAttended = 8,
                Speedup = 1.5,
                Efficiency = 0.75
            };
            await Context.SimulationRuns.AddAsync(run);
            await Context.SaveChangesAsync();

            // Act
            var result = await _service.GetSimulationResultAsync(run.Id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<SimulationResultViewModel>(result);
            Assert.Equal("Parallel", result.ExecutionMode);
            _output.WriteLine("GetSimulationResultAsync devolviendo las metricas.");
        }

        [Fact]
        public async Task GetUrgencyMetricsAsync_Should_ReturnMetricsForSimulation()
        {
            // Arrange
            int simulationId = 1;
            Context.UrgencyWaitMetrics.AddRange(
                new UrgencyWaitMetric
                {
                    SimulationRunId = simulationId,
                    UrgencyLevel = UrgencyLevel.HIGH,
                    TotalPatients = 3,
                    AverageWaitSeconds = 12.5
                },
                new UrgencyWaitMetric
                {
                    SimulationRunId = 999,
                    UrgencyLevel = UrgencyLevel.LOW,
                    TotalPatients = 5,
                    AverageWaitSeconds = 8
                }
            );
            Context.SaveChanges();

            // Act
            var metrics = await _service.GetUrgencyMetricsAsync(simulationId);

            // Assert
            Assert.Single(metrics);
            Assert.Equal("HIGH", metrics[0].UrgencyLevel);
            _output.WriteLine("GetUrgencyMetricsAsync devolviendo las metricas.");
        }

        [Fact]
        public async Task GetPerformanceMetricsAsync_Should_ReturnMetricsForSimulation()
        {
            // Arrange
            int simulationId = 2;
            Context.PerformanceMetrics.AddRange(
                new PerformanceMetric
                {
                    DoctorId = "D1",
                    SimulationRunId = simulationId,
                    PatientsAttended = 4,
                    AverageAttentionTimeSeconds = 15.67
                },
                new PerformanceMetric
                {
                    DoctorId = "D2",
                    SimulationRunId = 999,
                    PatientsAttended = 3,
                    AverageAttentionTimeSeconds = 12.33
                }
            );
            Context.SaveChanges();

            // Act
            var result = await _service.GetPerformanceMetricsAsync(simulationId);

            // Assert
            Assert.Single(result);
            Assert.Equal("D1", result[0].DoctorId);
            Assert.Equal(4, result[0].PatientsAttended);
            _output.WriteLine("GetPerformanceMetricsAsync devolviendo las metricas.");
        }

        [Fact]
        public async Task GetAllSimulationsAsync_Should_ReturnAllInDescendingOrder()
        {
            // Arrange
            var now = DateTime.UtcNow;
            Context.SimulationRuns.AddRange(
                new SimulationRun
                {
                    RunAt = now.AddMinutes(-10),
                    StrategyUsed = StrategyType.RoundRobin,
                    ProcessorsUsed = 1
                },
                new SimulationRun
                {
                    RunAt = now,
                    StrategyUsed = StrategyType.EmergencyType,
                    ProcessorsUsed = 2
                }
            );
            Context.SaveChanges();

            // Act
            var result = await _service.GetAllSimulationsAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.True(result[0].RunAt > result[1].RunAt);
            _output.WriteLine("GetAllSimulationsAsync devolviendo las metricas.");
        }
    }
}