using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearningPlatform.Entities;
using OnlineLearningPlatform.Enums;

namespace OnlineLearningPlatform.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses", t =>
            t.HasCheckConstraint("CK_Courses_Price_NonNegative", "[Price] >= 0"));

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Description).HasMaxLength(1000); // optional
        builder.Property(c => c.Price).IsRequired().HasPrecision(18, 2);

        // Enum stored as readable text instead of a number.
        builder.Property(c => c.Level)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(20);

        builder.Property(c => c.IsPublished).IsRequired();
        builder.Property(c => c.PublishedOn); // nullable: only set when published

        // Every course is published by exactly one instructor.
        builder.HasOne(c => c.Instructor)
               .WithMany(i => i.Courses)
               .HasForeignKey(c => c.InstructorId)
               .OnDelete(DeleteBehavior.Restrict);

        // Every course belongs to exactly one category.
        builder.HasOne(c => c.Category)
               .WithMany(cat => cat.Courses)
               .HasForeignKey(c => c.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

       /* builder.HasData(
            new Course
            {
                Id = 1, Title = "ASP.NET Core Fundamentals",
                Description = "Build your first web application with ASP.NET Core.",
                Price = 49.99m, Level = CourseLevel.Beginner, IsPublished = true,
                PublishedOn = new DateTime(2024, 1, 15), InstructorId = 1, CategoryId = 3
            },
            new Course
            {
                Id = 2, Title = "Advanced EF Core",
                Description = "Performance, concurrency and advanced mappings.",
                Price = 79.99m, Level = CourseLevel.Advanced, IsPublished = true,
                PublishedOn = new DateTime(2024, 3, 1), InstructorId = 1, CategoryId = 3
            },
            new Course
            {
                Id = 3, Title = "Building REST APIs with C#",
                Description = "Design and build clean RESTful APIs.",
                Price = 59.99m, Level = CourseLevel.Intermediate, IsPublished = true,
                PublishedOn = new DateTime(2024, 4, 10), InstructorId = 2, CategoryId = 2
            },
            new Course
            {
                Id = 4, Title = "Modern JavaScript",
                Description = null,
                Price = 39.99m, Level = CourseLevel.Beginner, IsPublished = true,
                PublishedOn = new DateTime(2024, 5, 5), InstructorId = 2, CategoryId = 4
            },
            new Course
            {
                Id = 5, Title = "UI Design Principles",
                Description = "Layout, color and typography basics.",
                Price = 29.99m, Level = CourseLevel.Beginner, IsPublished = false,
                PublishedOn = null, InstructorId = 3, CategoryId = 6
            },
            new Course
            {
                Id = 6, Title = "Design Systems in Practice",
                Description = "Create and maintain a scalable design system.",
                Price = 69.99m, Level = CourseLevel.Intermediate, IsPublished = false,
                PublishedOn = null, InstructorId = 3, CategoryId = 5
            });*/
    }
}
