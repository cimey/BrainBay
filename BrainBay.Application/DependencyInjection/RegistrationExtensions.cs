using BrainBay.Application.Features.Character;
using Microsoft.Extensions.DependencyInjection;
namespace BrainBay.Application.DependencyInjection
{
    public static class RegistrationExtensions
    {
        public static IServiceCollection RegisterApplication(this IServiceCollection services)
        {
            services.AddTransient<ICharacterService, CharacterService>();
            services.AddAutoMapper(typeof(RegistrationExtensions).Assembly);

            return services;
        }
    }
}
