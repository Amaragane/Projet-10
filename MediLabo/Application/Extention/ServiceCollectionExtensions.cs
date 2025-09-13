using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationMappings(this IServiceCollection services)
        {
            // Scanne automatiquement tous les profils dans l'assembly de Application
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
