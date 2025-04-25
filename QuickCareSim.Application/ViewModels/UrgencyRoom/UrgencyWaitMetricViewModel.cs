namespace QuickCareSim.Application.ViewModels.UrgencyRoom
{
    public class UrgencyWaitMetricViewModel
    {
        public string UrgencyLevel { get; set; } = string.Empty;
        public double AverageWaitSeconds { get; set; }
        public int TotalPatients { get; set; }
    }
}