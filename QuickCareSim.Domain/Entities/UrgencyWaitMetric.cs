using QuickCareSim.Domain.Enums;

namespace QuickCareSim.Domain.Entities
{
    public class UrgencyWaitMetric
    {
        public int Id { get; set; }
        public UrgencyLevel UrgencyLevel { get; set; }
        public double AverageWaitSeconds { get; set; }
        public int TotalPatients { get; set; }
        public int SimulationRunId { get; set; }
        public SimulationRun SimulationRun { get; set; } = null!;
    }
}