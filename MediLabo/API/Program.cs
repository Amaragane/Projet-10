    using Application;
    using Application.Extensions;
    using Infrastructure;
    using Infrastructure.Data;
    using Infrastructure.Extention;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
using System.Security.Cryptography;
using System.Text;


    var useSwagger = false;
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;
// Add services to the container.
    builder.Configuration.AddUserSecrets<Program>();

//Mise en place de l'infrastructure et la BDD
builder.Services.AddInfrastructure(configuration);

    builder.Services.AddServices();
    builder.Services.AddRepositories();
    builder.Services.AddControllers();
builder.Logging.AddConsole();

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
        .AddRoles<IdentityRole>()
         .AddEntityFrameworkStores<PatientDbContext>();
    var jwtSettings = configuration.GetSection("JwtSettings");
string publicKeyPem = await File.ReadAllTextAsync("/run/secrets/jwt_public_key");

// Créez une instance RSA à partir de la clé publique PEM
RSA rsa = RSA.Create();
rsa.ImportFromPem(publicKeyPem.ToCharArray());

var rsaSecurityKey = new RsaSecurityKey(rsa);


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
            IssuerSigningKey = rsaSecurityKey
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("JwtBearer");
                logger.LogError(ctx.Exception, "Authentication failed:");
                return Task.CompletedTask;
            }
        };

    });
builder.Services.AddSingleton<JwtTokenService>(sp =>
    new JwtTokenService(
        builder.Configuration,
        "/run/secrets/jwt_private_key"));  // chemin du secret Docker


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

    // Configure the HTTP request pipeline.
    if (useSwagger)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        });



    }
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PatientDbContext>();
    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erreur migration base : {ex.Message}");
        throw;
    }
}
    await SeedUserAsync(app.Services);
app.UseAuthentication();
    app.UseAuthorization();

    app.UseHttpsRedirection();


app.MapControllers();
    app.Run();

async Task SeedUserAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Création des rôles si n’existent pas
    if (await roleManager.FindByNameAsync("organisateur") == null)
        await roleManager.CreateAsync(new IdentityRole("organisateur"));
    if (await roleManager.FindByNameAsync("praticien") == null)
        await roleManager.CreateAsync(new IdentityRole("praticien"));

    // Utilisateur organisateur
    var userOrganisateur = await userManager.FindByNameAsync("organisateur");
    if (userOrganisateur == null)
    {
        userOrganisateur = new IdentityUser { UserName = "organisateur", Email = "organisateur@email.com" };
        await userManager.CreateAsync(userOrganisateur, "Password123!");
        await userManager.AddToRoleAsync(userOrganisateur, "organisateur");
    }

    // Utilisateur praticien
    var userPraticien = await userManager.FindByNameAsync("praticien");
    if (userPraticien == null)
    {
        userPraticien = new IdentityUser { UserName = "praticien", Email = "praticien@email.com" };
        await userManager.CreateAsync(userPraticien, "Password123!");
        await userManager.AddToRoleAsync(userPraticien, "praticien");
    }
}

