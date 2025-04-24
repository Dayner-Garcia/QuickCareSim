using QuickCareSim.Domain.Entities;
using System.Collections.Concurrent;

namespace QuickCareSim.Application.Interfaces.Services.Core
{
    public interface IPatientService
    {
        (ConcurrentQueue<string> queue, ConcurrentDictionary<string, Patient> patientDict) GeneratePatients(int total);
    }
}
