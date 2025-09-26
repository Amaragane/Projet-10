using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class PatientDbContextFactory : IDesignTimeDbContextFactory<PatientDbContext>
{
    public PatientDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PatientDbContext>();
        // Met ici ta chaîne de connexion
        optionsBuilder.UseSqlServer("Server=.;Database=PatientServiceDb;Trusted_Connection=True; TrustServerCertificate=True");
        return new PatientDbContext(optionsBuilder.Options);
    }
}
