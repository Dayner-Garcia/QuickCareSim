using QuickCareSim.Application.Interfaces.Repositories;
using QuickCareSim.Application.Interfaces.Services.Core;
using QuickCareSim.Domain.Entities;
using QuickCareSim.Domain.Enums;
using System.Collections.Concurrent;

namespace QuickCareSim.Application.Services.Core
{
    public class PatientService : IPatientService
    {
        private readonly IGenericRepository<Patient> _patientRepo;

        public PatientService(IGenericRepository<Patient> genericRepository)
        {
            _patientRepo = genericRepository;
        }

        public (ConcurrentQueue<string> queue, ConcurrentDictionary<string, Patient> patientDict)
            GeneratePatients(int total)
        {
            var queue = new ConcurrentQueue<string>();
            var patientDict = new ConcurrentDictionary<string, Patient>();
            var random = new ThreadLocal<Random>(() => new Random());

            Parallel.For(0, total, i =>
            {
                var urgency = (UrgencyLevel)random.Value!.Next(1, 5);
                var patient = new Patient
                {
                    UserId = Guid.NewGuid().ToString(),
                    ArrivalTime = DateTime.UtcNow,
                    Urgency = urgency,
                    Status = PatientStatus.WAITING
                };

                queue.Enqueue(patient.UserId);
                patientDict.TryAdd(patient.UserId, patient);
            });

            return (queue, patientDict);
        }
    }
}