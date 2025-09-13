using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Domain.Interfaces.ServicesInterfaces;

namespace Application
{
    public static class DependancyInjection
    {
        public static void AddServices(this IServiceCollection services)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            Assembly domainServicesAssembly = Assembly.Load("Domain");

            var assembliesToScan = new[] { currentAssembly, domainServicesAssembly };

            foreach (var assembly in assembliesToScan)
            {
                assembly.GetTypes()
                    .Where(t => !t.IsAbstract && !t.IsInterface && t.Name.EndsWith("Service"))
                    .Select(a => new { Implementation = a, ServiceTypes = a.GetInterfaces().ToList() })
                    .ToList()
                    .ForEach(tuple =>
                    {
                        tuple.ServiceTypes.ForEach(serviceType =>
                        {
                            services.AddScoped(serviceType, tuple.Implementation);
                        });
                    });
            }

        }
    }
}
