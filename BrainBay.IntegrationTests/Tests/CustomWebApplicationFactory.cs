using BrainBay.Application.DependencyInjection;
using BrainBay.Infrastructure.DatabaseContext;
using BrainBay.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BrainBay.IntegrationTests.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public readonly IConfiguration Configuration;

        public CustomWebApplicationFactory()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Replace DbContext to use test database
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<BrainBayDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                

                // Add InMemory database
                services.AddDbContext<BrainBayDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));

                services.RegisterInfra(Configuration);
                services.RegisterApplication();
            });
        }
    }

}
