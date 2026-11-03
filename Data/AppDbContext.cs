using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Entities;

namespace OnlineLearningPlatform.Data;

public class AppDbContext : DbContext
{
    public DbSet<Instructor> Instructors => Set<Instructor>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Override with the OLP_CONNECTION environment variable if you do not use LocalDB.
        var connectionString =
            Environment.GetEnvironmentVariable("OLP_CONNECTION")
          ?? @"Server=.;Database=OnlineLearningPlatformDb;Trusted_Connection=True;TrustServerCertificate=True";

        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Picks up every IEntityTypeConfiguration<T> in this assembly.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
