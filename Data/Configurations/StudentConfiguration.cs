using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearningPlatform.Entities;

namespace OnlineLearningPlatform.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.FullName).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Email).IsRequired().HasMaxLength(150);
        builder.Property(s => s.DateOfBirth).IsRequired().HasColumnType("date");

        builder.HasIndex(s => s.Email).IsUnique();

        // The Student <-> Course many-to-many relationship goes through Enrollment (see EnrollmentConfiguration).
    }
}
