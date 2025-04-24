namespace QuickCareSim.Application.ViewModels.UrgencyRoom
{
    public class SimulationResultViewModel
    {
        public int Id { get; set; }
        public DateTime RunAt { get; set; }
        public string StrategyUsed { get; set; } = string.Empty;
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int TotalPatientsAttended { get; set; }
        public double ExecutionTimeSeconds { get; set; }
        public double RealExecutionTimeSeconds { get; set; }
        public double TotalSimulationTimeSeconds { get; set; }
        public double PatientsPerMinute { get; set; }
        public int ProcessorsUsed { get; set; }
        public double? Speedup { get; set; }
        public double? Efficiency { get; set; }
        public string ExecutionMode { get; set; } = "Parallel";
    }
}