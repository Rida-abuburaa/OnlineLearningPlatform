using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearningPlatform.Entities;

namespace OnlineLearningPlatform.Data.Configurations;

public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.ToTable("Instructors");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.FullName).IsRequired().HasMaxLength(100);
        builder.Property(i => i.Email).IsRequired().HasMaxLength(150);
        builder.Property(i => i.Bio).HasMaxLength(500); // optional
        builder.Property(i => i.JoinedOn).IsRequired().HasColumnType("date");

        builder.HasIndex(i => i.Email).IsUnique();

        // The one-to-many Instructor -> Courses relationship is configured in CourseConfiguration.

        /* builder.HasData(
            new Instructor
            {
                Id = 1,
                FullName = "Sara Ahmed",
                Email = "sara.ahmed@learnhub.com",
                Bio = "Senior .NET developer and trainer with 10 years of experience.",
                JoinedOn = new DateTime(2023, 1, 10)
            },
            new Instructor
            {
                Id = 2,
                FullName = "Omar Khaled",
                Email = "omar.khaled@learnhub.com",
                Bio = "Full-stack engineer who loves teaching web development.",
                JoinedOn = new DateTime(2023, 6, 1)
            },
            new Instructor
            {
                Id = 3,
                FullName = "Laila Hassan",
                Email = "laila.hassan@learnhub.com",
                Bio = null,
                JoinedOn = new DateTime(2024, 2, 20)
            });*/
    }
}
