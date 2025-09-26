using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Data;
public class PatientDbContext : IdentityDbContext
{
    public PatientDbContext(DbContextOptions<PatientDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<IdentityRole>().HasData(
    new IdentityRole { Id = "1", Name = "organisateur", NormalizedName = "ORGANISATEUR" },
    new IdentityRole { Id = "2", Name = "praticien", NormalizedName = "PRATICIEN" }
);
        modelBuilder.Entity<Patient>().HasData(
            new Patient
            {
                Id = 1,
                FirstName = "Test",
                LastName = "None",
                DateOfBirth = new DateTime(1966, 12, 31),
                Gender = "F",
                Address = "1 Brookside St",
                PhoneNumber = "100-222-3333"
            },
            new Patient
            {
                Id = 2,
                FirstName = "Test",
                LastName = "Borderline",
                DateOfBirth = new DateTime(1945, 6, 24),
                Gender = "M",
                Address = "2 High St",
                PhoneNumber = "200-333-4444"
            },
            new Patient
            {
                Id = 3,
                FirstName = "Test",
                LastName = "InDanger",
                DateOfBirth = new DateTime(2004, 6, 18),
                Gender = "M",
                Address = "3 Club Road",
                PhoneNumber = "300-444-5555"
            },
            new Patient
            {
                Id = 4,
                FirstName = "Test",
                LastName = "EarlyOnset",
                DateOfBirth = new DateTime(2002, 6, 28),
                Gender = "F",
                Address = "4 Valley Dr",
                PhoneNumber = "400-555-6666"
            }
        );
    }

}
