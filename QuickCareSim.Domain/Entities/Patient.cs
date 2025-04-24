using QuickCareSim.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace QuickCareSim.Domain.Entities
{
    public class Patient
    {
       [Key] public string UserId { get; set; } = string.Empty;
       
       public UrgencyLevel Urgency { get; set; }
       public DateTime ArrivalTime { get; set; }
       public DateTime? AttendedTime { get; set; }
       public PatientStatus Status { get; set; }
       
       public string? DoctorId { get; set; }
       public Doctor? Doctor { get; set; }
    }
}