using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            //Ajouter les services
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            Assembly domainServicesAssembly = Assembly.Load("Domain");

            var assembliesToScan = new[] { currentAssembly, domainServicesAssembly };

            foreach (var assembly in assembliesToScan)
            {
                assembly.GetTypes()
                    .Where(t => !t.IsAbstract && !t.IsInterface && t.Name.EndsWith("Repository"))
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
