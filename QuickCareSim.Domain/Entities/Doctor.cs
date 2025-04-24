using QuickCareSim.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace QuickCareSim.Domain.Entities
{
    public class Doctor
    {
        [Key]
        public string UserId { get; set; } = string.Empty;
        
        public DoctorStatus Status { get; set; } = DoctorStatus.AVAILABLE;
        
        public ICollection<Patient> AttendedPatients { get; set; } = new List<Patient>();
        public ICollection<PerformanceMetric> Metrics { get; set; } = new List<PerformanceMetric>();
    }
}