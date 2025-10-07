using RiskLevelService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddScoped<IRiskService, RiskService>();
var app = builder.Build();


app.Run();
