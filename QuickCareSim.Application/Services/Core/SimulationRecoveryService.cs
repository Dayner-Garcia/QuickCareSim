using QuickCareSim.Application.Interfaces.Repositories;
using QuickCareSim.Application.Interfaces.Services.Core;
using QuickCareSim.Domain.Entities;
using QuickCareSim.Domain.Enums;

namespace QuickCareSim.Application.Services.Core
{
    public class SimulationRecoveryService : ISimulationRecoveryService
    {
        private readonly IGenericRepository<Doctor> _doctorRepository;

        public SimulationRecoveryService(IGenericRepository<Doctor> doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task ResetBusyDoctorsAsync()
        {
            var busyDoctors = await _doctorRepository.GetAllAsync(q =>
                q.Where(d => d.Status == DoctorStatus.BUSY));

            foreach (var doctor in busyDoctors)
            {
                doctor.Status = DoctorStatus.AVAILABLE;
                await _doctorRepository.UpdateAsync(doctor);
            }
        }
    }
}