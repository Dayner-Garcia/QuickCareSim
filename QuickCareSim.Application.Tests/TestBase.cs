using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickCareSim.Application.Interfaces.Repositories;
using QuickCareSim.Infrastructure.Persistance.Context;
using QuickCareSim.Infrastructure.Persistance.Repositories;

namespace QuickCareSim.Tests.Common
{
    public abstract class TestBase : IDisposable
    {
        protected readonly AppDbContext Context;
        protected readonly ServiceProvider Provider;

        protected TestBase()
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
                .Build();

            var useInMemory = configuration.GetValue<bool>("UseInMemoryDatabase");
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (useInMemory)
            {
                services.AddDbContext<AppDbContext>(opt =>
                    opt.UseInMemoryDatabase("TestDb"));
            }
            else
            {
                services.AddDbContext<AppDbContext>(opt =>
                    opt.UseSqlServer(connectionString));
            }

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            Provider = services.BuildServiceProvider();
            Context = Provider.GetRequiredService<AppDbContext>();

            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Provider.Dispose();
        }
    }
}
