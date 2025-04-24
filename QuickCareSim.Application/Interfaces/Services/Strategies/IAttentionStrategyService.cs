using QuickCareSim.Domain.Entities;
using System.Collections.Concurrent;

namespace QuickCareSim.Application.Interfaces.Services.Strategies
{
    public interface IAttentionStrategyService
    {
        Task ExecuteAsync(
            ConcurrentQueue<string> queue,
            ConcurrentDictionary<string, Patient> patientDict,
            int doctorIndex,
            int simulationRunId,
            Func<Patient, Task> onPatientAttended,
            CancellationToken token);
    }
}
