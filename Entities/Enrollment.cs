using OnlineLearningPlatform.Enums;

namespace OnlineLearningPlatform.Entities;


public class Enrollment
{
 
    public int StudentId { get; set; }
    public int CourseId { get; set; }

    public DateTime EnrolledOn { get; set; }
    public int ProgressPercentage { get; set; }
    public EnrollmentStatus Status { get; set; }

    
    public decimal? FinalGrade { get; set; }

    public Student Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
}
