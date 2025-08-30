using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extention
{
    public static class ServiceCollectionExtentions
    {

            public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
            {
                services.AddDbContext<PatientDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
                services.AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;
                })
                    .AddEntityFrameworkStores<PatientDbContext>();
        }
        
    }
}
