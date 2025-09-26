    using Application;
    using Application.Extensions;
    using Infrastructure;
    using Infrastructure.Data;
    using Infrastructure.Extention;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
using SolrNet;
using System.Security.Cryptography;
using System.Text;


    var useSwagger = false;
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;
// Add services to the container.
    builder.Configuration.AddUserSecrets<Program>();

    builder.Services.AddSingleton(new JwtTokenService(builder.Configuration));
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
    string privateKeyPem = builder.Configuration["JwtPrivateKey"];
var rsa = RSA.Create();

// Charger la clé publique PEM exportée depuis la privée
var rsaPublic = CreateRsaPublicKeyFromPem(privateKeyPem);


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
            IssuerSigningKey = new RsaSecurityKey(rsaPublic)
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
static RSA CreateRsaPublicKeyFromPem(string privateKeyPem)
{
    var rsa = RSA.Create();
    var base64 = privateKeyPem
        .Replace("-----BEGIN PRIVATE KEY-----", "")
        .Replace("-----END PRIVATE KEY-----", "")
        .Replace("\n", "")
        .Replace("\r", "")
        .Trim();
    var privateKeyBytes = Convert.FromBase64String(base64);
    rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);

    var publicKey = rsa.ExportSubjectPublicKeyInfo();
    var rsaPublic = RSA.Create();
    rsaPublic.ImportSubjectPublicKeyInfo(publicKey, out _);

    return rsaPublic;
}
