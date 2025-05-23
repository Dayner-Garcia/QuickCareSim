﻿

namespace QuickCareSim.Domain.Entities
{
    public class AttentionLog
    {
        public int Id { get; set; }
        
        public string PatientId { get; set; } = string.Empty;
        public string DoctorId { get; set; } = string.Empty;
        
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        
        public int SimulationRunId { get; set; }
        public SimulationRun SimulationRun { get; set; } = null!;
        
        public string StrategyUsed { get; set; } = string.Empty;
    }
}