using QuickCareSim.Application.Interfaces.Repositories;
using QuickCareSim.Application.Interfaces.Services.Core;
using QuickCareSim.Domain.Entities;
using QuickCareSim.Domain.Enums;

namespace QuickCareSim.Application.Services.Core
{
    public class DoctorService : IDoctorService
    {
        private readonly IGenericRepository<Doctor> _doctorRepository;

        public DoctorService(IGenericRepository<Doctor> doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<List<Doctor>> GetAvailableDoctorsAsync()
        {
            return await _doctorRepository.GetAllAsync(q =>
                q.Where(d => d.Status == DoctorStatus.AVAILABLE));
        }
    }
}