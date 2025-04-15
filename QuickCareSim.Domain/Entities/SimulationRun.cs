using QuickCareSim.Domain.Enums;

namespace QuickCareSim.Domain.Entities
{
    public class SimulationRun
    {
        public int Id { get; set; }
        public DateTime RunAt { get; set; }
        public StrategyType StrategyUsed { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public double ExecutionTimeSeconds { get; set; }
    }
}
