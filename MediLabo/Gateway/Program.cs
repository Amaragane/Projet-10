using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);


// Ajout d'Ocelot à partir de la configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
// Ajout d'un HttpClient personnalisé dans le container DI
//builder.Services.AddTransient<BypassSslValidationHandler>();

builder.Services.AddOcelot(builder.Configuration);
//.AddDelegatingHandler<BypassSslValidationHandler>(true); // true = global
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
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Trace);
});

var app = builder.Build();

app.UseCors();
// Utilisation de la middleware Ocelot
await app.UseOcelot();

app.MapGet("/", () => "Hello World!");

// Ajoutez uniquement un url à 'app.Urls', soit http soit https, pas les deux ici
app.Urls.Add("https://localhost:5002");

app.Run();
