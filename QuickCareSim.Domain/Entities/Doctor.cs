
using System.ComponentModel.DataAnnotations;

namespace QuickCareSim.Domain.Entities
{
    public class Doctor
    {
        [Key]
        public string UserId { get; set; } = string.Empty;

        public ICollection<Patient> AttendedPatients { get; set; } = new List<Patient>();
    }
}
