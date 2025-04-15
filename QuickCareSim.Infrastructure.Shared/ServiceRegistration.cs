using Microsoft.Extensions.DependencyInjection;
using QuickCareSim.Infrastructure.Shared.Interfaces;
using QuickCareSim.Infrastructure.Shared.Services;

namespace QuickCareSim.Infrastructure.Shared
{
    public static class ServiceRegistration
    {
        public static void AddSharedService(this IServiceCollection services)
        {
            services.AddScoped<IEmailService, EmailService>();
        }
    }
}
