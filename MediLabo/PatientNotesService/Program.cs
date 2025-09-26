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

// Charger la cl� publique PEM export�e depuis la priv�e
var rsaPublic = CreateRsaPublicKeyFromPem(privateKeyPem);
// Configure JWT authentication comme dans ton autre service SQL,
// avec la cl� publique RSA et validation des audiences et issuer.
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