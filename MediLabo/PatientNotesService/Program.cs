using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Services;
using System.Security.Cryptography;
using DomainNote = PatientNotesService.Domain.Note;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
var mongoSettings = builder.Configuration.GetSection("MongoSettings");
string mongoConnStr = mongoSettings["ConnectionString"]!;
string mongoDbName = mongoSettings["DatabaseName"]!;

builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoConnStr));
builder.Services.AddSingleton<IMongoDatabase>(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDbName));
builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoSettings"));

builder.Services.AddScoped<NotesService>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();

var jwtSettings = configuration.GetSection("JwtSettings");
string publicKeyPem = await File.ReadAllTextAsync("/run/secrets/jwt_public_key");

// Cr�ez une instance RSA � partir de la cl� publique PEM
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



var app = builder.Build();
await SeedNotesAsync(app.Services);
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


async Task SeedNotesAsync(IServiceProvider services)
{
    var defaultNotes = new List<DomainNote>
    {
        new DomainNote { PatId = 1, Content = "Le patient d�clare qu'il 'se sent tr�s bien' Poids �gal ou inf�rieur au poids recommand�" },
        new DomainNote { PatId = 2, Content = "Le patient d�clare qu'il ressent beaucoup de stress au travail Il se plaint �galement que son audition est anormale derni�rement" },
        new DomainNote { PatId = 2, Content = "Le patient d�clare avoir fait une r�action aux m�dicaments au cours des 3 derniers mois Il remarque �galement que son audition continue d'�tre anormale" },
        new DomainNote { PatId = 3, Content = "Le patient d�clare qu'il fume depuis peu" },
        new DomainNote { PatId = 3, Content = "Le patient d�clare qu'il est fumeur et qu'il a cess� de fumer l'ann�e derni�re Il se plaint �galement de crises d�apn�e respiratoire anormales Tests de laboratoire indiquant un taux de cholest�rol LDL �lev�" },
        new DomainNote { PatId = 4, Content = "Le patient d�clare qu'il lui est devenu difficile de monter les escaliers Il se plaint �galement d��tre essouffl� Tests de laboratoire indiquant que les anticorps sont �lev�s R�action aux m�dicaments" },
        new DomainNote { PatId = 4, Content = "Le patient d�clare qu'il a mal au dos lorsqu'il reste assis pendant longtemps" },
        new DomainNote { PatId = 4, Content = "Le patient d�clare avoir commenc� � fumer depuis peu H�moglobine A1C sup�rieure au niveau recommand�" },
        new DomainNote { PatId = 4, Content = "Taille, Poids, Cholest�rol, Vertige et R�action" }
    };
    using var scope = services.CreateScope();
    var notesService = scope.ServiceProvider.GetRequiredService<NotesService>();
    var existingNotes = await notesService.GetAllAsync();

    if (!existingNotes.Any())
        await notesService.InsertManyAsync(defaultNotes);
}
