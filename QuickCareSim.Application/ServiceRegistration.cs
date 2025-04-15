using Microsoft.Extensions.DependencyInjection;
using QuickCareSim.Application.Interfaces.Services;
using QuickCareSim.Application.Mappings;
using QuickCareSim.Application.Services;
using System.Reflection;

namespace QuickCareSim.Application
{
    public static class ServiceRegistration
    {
        public static void AddServicesForWebApp(this IServiceCollection services)
        {

            #region "GenericService"

            services.AddScoped(typeof(IGenericService<,,,>), typeof(GenericService<,,,>));

            #endregion

            #region Patients

            #endregion


            #region "Helpers"


            #endregion

            #region "AutoMapper"

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(typeof(GeneralProfile));

            #endregion
        }
    }
}
