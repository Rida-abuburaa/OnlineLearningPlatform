using OnlineLearningPlatform.Enums;

namespace OnlineLearningPlatform.Entities;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public CourseLevel Level { get; set; }
    public bool IsPublished { get; set; }


    public DateTime? PublishedOn { get; set; }

    public int InstructorId { get; set; }
    public Instructor Instructor { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
