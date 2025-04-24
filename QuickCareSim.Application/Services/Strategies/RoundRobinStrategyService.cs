using QuickCareSim.Application.Interfaces.Services.Strategies;
using QuickCareSim.Application.Utils;
using QuickCareSim.Domain.Entities;
using System.Collections.Concurrent;

namespace QuickCareSim.Application.Services.Strategys
{
    public class RoundRobinStrategyService : IAttentionStrategyService
    {
        public async Task ExecuteAsync(
            ConcurrentQueue<string> queue,
            ConcurrentDictionary<string, Patient> patientDict,
            int doctorIndex,
            int simulationRunId,
            Func<Patient, Task> onPatientAttended,
            CancellationToken token)
        {
            while (!token.IsCancellationRequested && queue.TryDequeue(out var id))
            {
                if (!patientDict.TryGetValue(id, out var patient)) continue;

                await PatientAttendingHelper.AttendAsync(patient, onPatientAttended, token);
            }
        }
    }
}