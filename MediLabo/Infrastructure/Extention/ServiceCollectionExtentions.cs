using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure.Extention
{
    public static class ServiceCollectionExtentions
    {

            public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
            {
                services.AddDbContext<PatientDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly("Infrastructure")));
 
        }
        
    }
}
