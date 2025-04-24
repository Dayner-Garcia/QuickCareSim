

namespace QuickCareSim.Application.ViewModels.UrgencyRoom
{
    public class PerformanceMetricViewModel
    {
        public string DoctorId { get; set; } = string.Empty;
        public int PatientsAttended { get; set; }
        public double AverageAttentionTimeSeconds { get; set; }
    }
}
