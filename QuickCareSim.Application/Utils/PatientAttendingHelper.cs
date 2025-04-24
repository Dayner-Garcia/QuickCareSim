using QuickCareSim.Domain.Entities;
using QuickCareSim.Domain.Enums;

namespace QuickCareSim.Application.Utils
{
    public static class PatientAttendingHelper
    {
        public static async Task AttendAsync(Patient patient, Func<Patient, Task> onPatientAttended,
            CancellationToken token)
        {
            patient.Status = PatientStatus.IN_ATTENTION;
            patient.AttendedTime = DateTime.UtcNow;

            await Task.Delay(UrgencyUtils.GetSimulatedAttentionTime(patient.Urgency) * 1000, token);

            patient.Status = PatientStatus.ATTENDED;
            await onPatientAttended(patient);
        }
    }
}