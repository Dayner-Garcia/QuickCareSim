using QuickCareSim.Application.Utils;
using QuickCareSim.Domain.Entities;
using System.Collections.Concurrent;
using QuickCareSim.Application.Interfaces.Services.Strategies;

namespace QuickCareSim.Application.Services.Strategys
{
    public class PriorityStrategyService : IAttentionStrategyService
    {
        private readonly object _lock = new();

        public async Task ExecuteAsync(
            ConcurrentQueue<string> queue,
            ConcurrentDictionary<string, Patient> patientDict,
            int doctorIndex,
            int simulationRunId,
            Func<Patient, Task> onPatientAttended,
            CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Patient? target = null;
                string? targetId = null;

                lock (_lock)
                {
                    var snapshot = new List<(string id, Patient p)>();
                    foreach (var id in queue)
                    {
                        if (patientDict.TryGetValue(id, out var p))
                        {
                            snapshot.Add((id, p));
                        }
                    }

                    var sorted = snapshot.OrderByDescending(x => x.p.Urgency).ToList();
                    if (sorted.Any())
                    {
                        targetId = sorted.First().id;
                        target = sorted.First().p;

                        var tempQueue = new Queue<string>();
                        while (queue.TryDequeue(out var id))
                        {
                            if (id != targetId)
                                tempQueue.Enqueue(id);
                            else
                                break;
                        }

                        while (tempQueue.Count > 0)
                            queue.Enqueue(tempQueue.Dequeue());
                    }
                }

                if (target != null)
                {
                    await PatientAttendingHelper.AttendAsync(target, onPatientAttended, token);
                }
                else
                {
                    await Task.Yield();
                }

                if (queue.IsEmpty)
                    break;
            }
        }
    }
}
