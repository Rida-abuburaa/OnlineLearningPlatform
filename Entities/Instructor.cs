namespace OnlineLearningPlatform.Entities;

public class Instructor
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Bio { get; set; }
    public DateTime JoinedOn { get; set; }

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
