
namespace QuickCareSim.Domain.Entities
{
    public class PerformanceMetric
    {
        public int Id { get; set; }
        public string DoctorId { get; set; } = string.Empty;
        public Doctor Doctor { get; set; } = null!;
        public int PatientsAttended { get; set; }
        public double AverageAttentionTimeSeconds { get; set; }
    }
}
