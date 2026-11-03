using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearningPlatform.Entities;

namespace OnlineLearningPlatform.Data.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments", t =>
        {
            t.HasCheckConstraint("CK_Enrollments_Progress_Range", "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");
            t.HasCheckConstraint("CK_Enrollments_FinalGrade_Range", "[FinalGrade] IS NULL OR ([FinalGrade] >= 0 AND [FinalGrade] <= 100)");
        });

        // Composite key: also guarantees a student can enroll in the same course only once.
        builder.HasKey(e => new { e.StudentId, e.CourseId });

        builder.Property(e => e.EnrolledOn).IsRequired();
        builder.Property(e => e.ProgressPercentage).IsRequired();

        builder.Property(e => e.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(20);

        builder.Property(e => e.FinalGrade).HasPrecision(5, 2); // optional

        builder.HasOne(e => e.Student)
               .WithMany(s => s.Enrollments)
               .HasForeignKey(e => e.StudentId)
               .OnDelete(DeleteBehavior.Cascade);

        // Cascade: when a course is deleted, the database also deletes its enrollments.
        builder.HasOne(e => e.Course)
               .WithMany(c => c.Enrollments)
               .HasForeignKey(e => e.CourseId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
