﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    //if (existingNotes.Any())
    //{
    //    foreach (var note in existingNotes)
    //        await notesService.DeleteAsync(note.Id);
    //    await notesService.InsertManyAsync(defaultNotes);

    //}

    if (!existingNotes.Any())
        await notesService.InsertManyAsync(defaultNotes);
}
