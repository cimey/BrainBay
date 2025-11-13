using BrainBay.Core.Repositories;
using BrainBay.Infrastructure.DatabaseContext;
using BrainBay.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BrainBay.Infrastructure.DependencyInjection
{
    public static class RegistrationExtensions
    {

        public static IServiceCollection RegisterInfra(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<BrainBayDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Default")));

            services.AddTransient<ICharacterRepository, CharacterRepository>();
            services.AddTransient<ICharacterRepositoryDecorator, CachedCharacterRepository>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = $"{configuration["Redis:Host"]}:{configuration["Redis:Port"]}";
                options.InstanceName = configuration["Redis:InstanceName"];
            });

            return services;
        }
    }
}
