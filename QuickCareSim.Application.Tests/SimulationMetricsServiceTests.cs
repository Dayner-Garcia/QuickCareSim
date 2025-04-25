using Microsoft.Extensions.DependencyInjection;
using QuickCareSim.Application.Interfaces.Repositories;
using QuickCareSim.Application.Services.Core;
using QuickCareSim.Domain.Entities;
using QuickCareSim.Domain.Enums;
using QuickCareSim.Tests.Common;
using Xunit.Abstractions;

namespace QuickCareSim.Tests.Application
{
    public class SimulationMetricsServiceTests : TestBase
    {
        private readonly SimulationMetricsService _service;
        private readonly ITestOutputHelper _output;

        public SimulationMetricsServiceTests(ITestOutputHelper output)
        {
            _output = output;

            _service = new SimulationMetricsService(
                Provider.GetRequiredService<IGenericRepository<UrgencyWaitMetric>>(),
                Provider.GetRequiredService<IGenericRepository<PerformanceMetric>>(),
                Provider.GetRequiredService<IGenericRepository<SimulationRun>>()
            );
        }

        [Fact]
        public async Task StoreUrgencyMetricsAsync_Should_CalculateMetricsByUrgency()
        {
            // Arrange
            int runId = 1;
            var patients = new List<Patient>
            {
                new Patient
                {
                    ArrivalTime = DateTime.UtcNow.AddSeconds(-10),
                    AttendedTime = DateTime.UtcNow,
                    Urgency = UrgencyLevel.HIGH
                },
                new Patient
                {
                    ArrivalTime = DateTime.UtcNow.AddSeconds(-20),
                    AttendedTime = DateTime.UtcNow,
                    Urgency = UrgencyLevel.HIGH
                },
                new Patient
                {
                    ArrivalTime = DateTime.UtcNow.AddSeconds(-30),
                    AttendedTime = DateTime.UtcNow,
                    Urgency = UrgencyLevel.LOW
                }
            };

            // Act
            await _service.StoreUrgencyMetricsAsync(patients, runId);

            // Assert
            var metrics = Context.UrgencyWaitMetrics.Where(m => m.SimulationRunId == runId).ToList();
            Assert.Equal(2, metrics.Count);
            Assert.All(metrics, m => Assert.True(m.TotalPatients > 0));

            _output.WriteLine("StoreUrgencyMetricsAsync generrra correctamente las metricas por urgencia.");
        }

        [Fact]
        public async Task StorePerformanceMetricsAsync_Should_SaveMetricsGroupedByDoctor()
        {
            // Arrange
            int runId = 2;
            var logs = new List<AttentionLog>
            {
                new AttentionLog
                {
                    DoctorId = "D1",
                    StartTime = DateTime.UtcNow.AddSeconds(-15),
                    EndTime = DateTime.UtcNow,
                    SimulationRunId = runId
                },
                new AttentionLog
                {
                    DoctorId = "D1",
                    StartTime = DateTime.UtcNow.AddSeconds(-30),
                    EndTime = DateTime.UtcNow.AddSeconds(-10),
                    SimulationRunId = runId
                },
                new AttentionLog
                {
                    DoctorId = "D2",
                    StartTime = DateTime.UtcNow.AddSeconds(-25),
                    EndTime = DateTime.UtcNow,
                    SimulationRunId = runId
                }
            };

            // Act
            await _service.StorePerformanceMetricsAsync(logs, runId);

            // Assert
            var metrics = Context.PerformanceMetrics.Where(m => m.SimulationRunId == runId).ToList();
            Assert.Equal(2, metrics.Count);
            Assert.All(metrics, m => Assert.True(m.PatientsAttended > 0));

            _output.WriteLine("StorePerformanceMetricsAsync guardo las metricas por ell doctor.");
        }

        [Fact]
        public async Task UpdateSpeedupAndEfficiencyAsync_Should_CalculateCorrectValues()
        {
            // Arrange
            var run = new SimulationRun
            {
                Id = 3,
                ExecutionTimeSeconds = 5,
                ProcessorsUsed = 2
            };
            await Context.SimulationRuns.AddAsync(run);
            await Context.SaveChangesAsync();

            // Act
            await _service.UpdateSpeedupAndEfficiencyAsync(run, tiempoSecuencial: 10);

            // Assert
            var updated = await Context.SimulationRuns.FindAsync(3);
            Assert.Equal(2, updated.Speedup);
            Assert.Equal(1, updated.Efficiency);

            _output.WriteLine("UpdateSpeedupAndEfficiencyAsync calcula speedup y eficiencia correctamente.");
        }

        [Fact]
        public async Task UpdateSpeedupAndEfficiencyAsync_Should_SetNegative_When_InvalidInput()
        {
            // Arrange
            var run = new SimulationRun
            {
                Id = 4,
                ExecutionTimeSeconds = 0,
                ProcessorsUsed = 1
            };
            await Context.SimulationRuns.AddAsync(run);
            await Context.SaveChangesAsync();

            // Act
            await _service.UpdateSpeedupAndEfficiencyAsync(run, tiempoSecuencial: 10);

            // Assert
            var updated = await Context.SimulationRuns.FindAsync(4);
            Assert.Equal(-1, updated.Speedup);
            Assert.Equal(-1, updated.Efficiency);

            _output.WriteLine("UpdateSpeedupAndEfficiencyAsync manejo correctamente los datos invalido.");
        }
    }
}