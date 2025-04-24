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
        public double RealExecutionTimeSeconds { get; set; }
        
        public int TotalPatientsAttended { get; set; }
        public int ProcessorsUsed { get; set; }
        public bool MetricsFinalized { get; set; } = false;
        
        public double PatientsPerMinute =>
            RealExecutionTimeSeconds == 0 ? 0 : TotalPatientsAttended / (RealExecutionTimeSeconds / 60);
        
        public ICollection<PerformanceMetric> PerformanceMetrics { get; set; } = new List<PerformanceMetric>();
        public ICollection<UrgencyWaitMetric> UrgencyWaitMetrics { get; set; } = new List<UrgencyWaitMetric>();
        public ICollection<AttentionLog> AttentionLogs { get; set; } = new List<AttentionLog>();
        
        public double? Speedup { get; set; }
        public double? Efficiency { get; set; }
    }
}