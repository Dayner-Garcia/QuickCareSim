using Microsoft.Extensions.DependencyInjection;
using QuickCareSim.Application.Interfaces.Services;
using QuickCareSim.Application.Interfaces.Services.Core;
using QuickCareSim.Application.Interfaces.Services.Executors;
using QuickCareSim.Application.Interfaces.Services.Strategies;
using QuickCareSim.Application.Mappings;
using QuickCareSim.Application.Services.Core;
using QuickCareSim.Application.Services.Executors;
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

            #region Services

            #region Simulator

            services.AddScoped<ISimulationMetricsService, SimulationMetricsService>();
            services.AddScoped<ISimulationInfoService, SimulationInfoService>();
            services.AddScoped<ISimulationRetryHandler, SimulationRetryHandler>();
            services.AddScoped<ISimulationRecoveryService, SimulationRecoveryService>();
            services.AddScoped<IParallelSimulationExecutor, ParallelSimulationExcecutor>();
            services.AddScoped<ISequentialSimulationExecutor, SequentialSimulationExecutor>();
            services.AddScoped<ISpeedupCalculator, SpeedupCalculator>();
            services.AddScoped<IAttentionStrategyFactoryService, AttentionStrategyFactoryService>();

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

            services.AddScoped<IExportMetricsService, ExportMetricsService>();

            #endregion

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