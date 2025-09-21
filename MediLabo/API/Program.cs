    using Application;
    using Application.Extensions;
    using Infrastructure;
    using Infrastructure.Data;
    using Infrastructure.Extention;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using System.Text;


    var useSwagger = false;
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;
    // Add services to the container.


    //Mise en place de l'infrastructure et la BDD
    builder.Services.AddInfrastructure(configuration);
    builder.Services.AddServices();
    builder.Services.AddRepositories();

    builder.Services.AddControllers();

    builder.Services.AddApplicationMappings();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
if (useSwagger)
{
        builder.Services.AddSwaggerGen();
    
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Veuillez saisir un token JWT. Exemple : Bearer {votre token}",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    new string[] {}
                }
            });
            });
}

    builder.Services.AddIdentityApiEndpoints<IdentityUser>()
         .AddEntityFrameworkStores<PatientDbContext>();
    var jwtSettings = configuration.GetSection("JwtSettings");
    var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
    options.TokenValidationParameters = new TokenValidationParameters
    {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = jwtSettings["Issuer"],
    ValidAudience = jwtSettings["Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(key)
    };
    });
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();
    app.UseCors();
    await SeedUserAsync(app.Services);

    // Configure the HTTP request pipeline.
    if (useSwagger)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        });



    }
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseHttpsRedirection();


app.MapControllers();
    //app.Urls.Add("https://localhost:5001");
    app.Run();

    async Task SeedUserAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        if (await userManager.FindByNameAsync("demo") == null)
        {
            var user = new IdentityUser { UserName = "demo", Email = "demo@email.com" };
            await userManager.CreateAsync(user, "Password123!");
        }
    }