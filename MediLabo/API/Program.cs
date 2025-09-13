using Application;
using Application.Extensions;
using Infrastructure;
using Infrastructure.Extention;
using Microsoft.AspNetCore.Identity;


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
builder.Services.AddSwaggerGen();
var app = builder.Build();
await SeedUserAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
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
app.Urls.Add("https://localhost:5001");
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