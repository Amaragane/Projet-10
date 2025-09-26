using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services;
using System.Security.Cryptography;
using DomainNote = PatientNotesService.Domain.Note;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoSettings"));
builder.Services.AddSingleton<NotesService>();
var jwtSettings = configuration.GetSection("JwtSettings");
string privateKeyPem = builder.Configuration["JwtPrivateKey"];
var rsa = RSA.Create();

// Charger la clé publique PEM exportée depuis la privée
var rsaPublic = CreateRsaPublicKeyFromPem(privateKeyPem);
// Configure JWT authentication comme dans ton autre service SQL,
// avec la clé publique RSA et validation des audiences et issuer.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtIssuer"],
            ValidAudience = builder.Configuration["JwtAudience"],
            IssuerSigningKey = new RsaSecurityKey(rsaPublic)
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
        new DomainNote { PatId = 1, Content = "Le patient déclare qu'il 'se sent très bien' Poids égal ou inférieur au poids recommandé" },
        new DomainNote { PatId = 2, Content = "Le patient déclare qu'il ressent beaucoup de stress au travail Il se plaint également que son audition est anormale dernièrement" },
        new DomainNote { PatId = 2, Content = "Le patient déclare avoir fait une réaction aux médicaments au cours des 3 derniers mois Il remarque également que son audition continue d'être anormale" },
        new DomainNote { PatId = 3, Content = "Le patient déclare qu'il fume depuis peu" },
        new DomainNote { PatId = 3, Content = "Le patient déclare qu'il est fumeur et qu'il a cessé de fumer l'année dernière Il se plaint également de crises d’apnée respiratoire anormales Tests de laboratoire indiquant un taux de cholestérol LDL élevé" },
        new DomainNote { PatId = 4, Content = "Le patient déclare qu'il lui est devenu difficile de monter les escaliers Il se plaint également d’être essoufflé Tests de laboratoire indiquant que les anticorps sont élevés Réaction aux médicaments" },
        new DomainNote { PatId = 4, Content = "Le patient déclare qu'il a mal au dos lorsqu'il reste assis pendant longtemps" },
        new DomainNote { PatId = 4, Content = "Le patient déclare avoir commencé à fumer depuis peu Hémoglobine A1C supérieure au niveau recommandé" },
        new DomainNote { PatId = 4, Content = "Taille, Poids, Cholestérol, Vertige et Réaction" }
    };
    using var scope = services.CreateScope();
    var notesService = scope.ServiceProvider.GetRequiredService<NotesService>();
    var existingNotes = await notesService.GetAllAsync();

    if (!existingNotes.Any())
        await notesService.InsertManyAsync(defaultNotes);
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