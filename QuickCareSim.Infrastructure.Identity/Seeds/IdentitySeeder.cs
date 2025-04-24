using Microsoft.AspNetCore.Identity;
using QuickCareSim.Application.Interfaces.Repositories;
using QuickCareSim.Domain.Entities;
using QuickCareSim.Domain.Enums;
using QuickCareSim.Infrastructure.Identity.Entities;

namespace QuickCareSim.Infrastructure.Identity.Seeds
{
    public class IdentitySeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IGenericRepository<Doctor> doctorRepository)
        {
            var roles = Enum.GetValues(typeof(Roles))
                .Cast<Roles>()
                .Select(r => r.ToString());

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Usuarios de prueba
            var predefinedUsers =
                new List<(string Email, string Username, string Name, string Lastname, string Password, Roles Role)>
                {
                    #region Doctores

                    ("doctor1@gmail.com", "doctor1", "John", "Doe", "Doctor123@", Roles.DOCTOR),
                    ("doctor2@gmail.com", "doctor2", "Alice", "Johnson", "Doctor123@", Roles.DOCTOR),
                    ("doctor3@gmail.com", "doctor3", "Carlos", "Ramírez", "Doctor123@", Roles.DOCTOR),
                    ("doctor4@gmail.com", "doctor4", "Marta", "González", "Doctor123@", Roles.DOCTOR),
                    ("doctor5@gmail.com", "doctor5", "Luis", "Torres", "Doctor123@", Roles.DOCTOR),
                    ("doctor6@gmail.com", "doctor6", "Ana", "Castro", "Doctor123@", Roles.DOCTOR),
                    ("doctor7@gmail.com", "doctor7", "John", "Doe", "Doctor123@", Roles.DOCTOR),
                    ("doctor8@gmail.com", "doctor8", "Alice", "Johnson", "Doctor123@", Roles.DOCTOR),
                    ("doctor9@gmail.com", "doctor9", "Carlos", "Ramírez", "Doctor123@", Roles.DOCTOR),
                    ("doctor10@gmail.com", "doctor10", "Marta", "González", "Doctor123@", Roles.DOCTOR),
                    ("doctor11@gmail.com", "doctor11", "Luis", "Torres", "Doctor123@", Roles.DOCTOR),
                    ("doctor12@gmail.com", "doctor12", "Ana", "Castro", "Doctor123@", Roles.DOCTOR),
                    ("doctor13@gmail.com", "doctor13", "Alice", "Johnson", "Doctor123@", Roles.DOCTOR),
                    ("doctor14@gmail.com", "doctor14", "Carlos", "Ramírez", "Doctor123@", Roles.DOCTOR),
                    ("doctor15@gmail.com", "doctor15", "Marta", "González", "Doctor123@", Roles.DOCTOR),
                    ("doctor16@gmail.com", "doctor16", "Luis", "Torres", "Doctor123@", Roles.DOCTOR),

                    #endregion

                    #region Pacientes

                    #endregion

                    #region Secretaria(o)

                    #endregion
                };

            foreach (var (email, username, name, lastname, password, role) in predefinedUsers)
            {
                var existingUser = await userManager.FindByEmailAsync(email);
                if (existingUser == null)
                {
                    var user = new ApplicationUser
                    {
                        Email = email,
                        UserName = username,
                        Name = name,
                        LastName = lastname,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, role.ToString());

                        if (role == Roles.DOCTOR)
                        {
                            var doctor = new Doctor
                            {
                                UserId = user.Id,
                                Status = DoctorStatus.AVAILABLE
                            };
                            await doctorRepository.AddAsync(doctor);
                        }
                    }
                }
            }
        }
    }
}