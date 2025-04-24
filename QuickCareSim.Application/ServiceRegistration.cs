using Microsoft.Extensions.DependencyInjection;
using QuickCareSim.Application.Interfaces.Services.Core;
using QuickCareSim.Application.Interfaces.Services.Executors;
using QuickCareSim.Application.Mappings;
using QuickCareSim.Application.Services.Core;
using QuickCareSim.Application.Services.Executors;
using QuickCareSim.Application.Services.Strategies;
using QuickCareSim.Application.Services.Strategys;
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

            #region Simulator

            services.AddScoped<ISimulationMetricsService, SimulationMetricsService>();
            services.AddScoped<ISimulationInfoService, SimulationInfoService>();


            services.AddScoped<ISequentialSimulationExecutor, SequentialSimulationExecutor>();
            services.AddScoped<ISpeedupCalculator, SpeedupCalculator>();

            #endregion

            #region Strategy

            services.AddScoped<RoundRobinStrategyService>();
            services.AddScoped<PriorityStrategyService>();
            services.AddScoped<EmergencyTypeStrategyService>();

            #endregion

            #region Users


            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IPatientService, PatientService>();

            #endregion

            #region exportsFiles


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
