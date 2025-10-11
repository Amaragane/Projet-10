using RiskLevelService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddScoped<IRiskService, RiskService>();
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
builder.Services.AddControllers();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
var app = builder.Build();
app.UseHttpsRedirection();

app.MapControllers();
app.Run();
