using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickCareSim.Application.Interfaces.Repositories;
using QuickCareSim.Infrastructure.Persistance.Context;
using QuickCareSim.Infrastructure.Persistance.Repositories;

namespace QuickCareSim.Infrastructure.Persistance
{
    public static class ServiceRegistration
    {
        public static void AddContextInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<AppDbContext>(options => { options.UseInMemoryDatabase("ContextDb"); });
            }
            else
            {
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.EnableSensitiveDataLogging();
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                        mbox => mbox.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
                });
            }
        }
    }
}