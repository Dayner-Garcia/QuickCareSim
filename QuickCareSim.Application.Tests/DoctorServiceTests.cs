using QuickCareSim.Application.Services.Core;
using QuickCareSim.Domain.Entities;
using QuickCareSim.Domain.Enums;
using QuickCareSim.Tests.Common;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using QuickCareSim.Application.Interfaces.Repositories;

namespace QuickCareSim.Tests.Application
{
    public class DoctorServiceTests : TestBase
    {
        private readonly DoctorService _service;
        private readonly ITestOutputHelper _output;

        public DoctorServiceTests(ITestOutputHelper output)
        {
            _output = output;
            _service = new DoctorService(
                Provider.GetRequiredService<IGenericRepository<Doctor>>());
            
        }

        [Fact]
        public async Task GetAvailableDoctorsAsync_ShouldReturnOnlyAvailableDoctors()
        {
            // Arrange
            Context.Doctors.AddRange(
                new Doctor { UserId = "1", Status = DoctorStatus.AVAILABLE },
                new Doctor { UserId = "2", Status = DoctorStatus.BUSY },
                new Doctor { UserId = "3", Status = DoctorStatus.AVAILABLE }
            );
            Context.SaveChanges();

            // Act
            var result = await _service.GetAvailableDoctorsAsync();

            // Assert
            Assert.NotEmpty(result);
            Assert.All(result, d => Assert.Equal(DoctorStatus.AVAILABLE, d.Status));
            Assert.Equal(2, result.Count);

            _output.WriteLine("Test ppara optener los docutores correctamente.");
        }
    }
}
