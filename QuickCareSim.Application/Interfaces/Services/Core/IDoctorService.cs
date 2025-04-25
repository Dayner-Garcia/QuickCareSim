using QuickCareSim.Domain.Entities;

namespace QuickCareSim.Application.Interfaces.Services.Core
{
    public interface IDoctorService
    {
        Task<List<Doctor>> GetAvailableDoctorsAsync();
    }
}
