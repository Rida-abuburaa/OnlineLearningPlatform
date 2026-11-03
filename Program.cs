using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Data;
using OnlineLearningPlatform.Entities;
using OnlineLearningPlatform.Enums;

namespace OnlineLearningPlatform;

public static class Program
{
    
    private const string AhmedEmail = "ahmed.samir@example.com";
    private const string MariamEmail = "mariam.ali@example.com";
    private const string YoussefEmail = "youssef.nabil@example.com";

    public static void Main()
    {
        try
        {
            
            using (var db = new AppDbContext())
            {
                db.Database.Migrate();
            }

            SeedStudentsAndEnrollments();

            
            Header("CREATE");
            int instructorId = AddInstructor("Nour Mostafa", "nour.mostafa@example.com",
                "Cloud and DevOps trainer.", new DateTime(2026, 1, 15));

            int newCourseId = AddCourse("Introduction to Cloud Computing", "Cloud basics for developers.",
                99m, CourseLevel.Beginner, instructorId, categoryId: 3);
            AddCourse("Ghost Course", null, 10m, CourseLevel.Beginner, instructorId: 999, categoryId: 3);   
            AddCourse("Ghost Course", null, 10m, CourseLevel.Beginner, instructorId, categoryId: 999);      

            List<int> newStudentIds = AddStudents();
            int newStudentId = newStudentIds.Count > 0 ? newStudentIds[0] : 0;

            EnrollStudent(newStudentId, newCourseId);                       
            EnrollStudent(StudentIdByEmail(AhmedEmail), 1);                 
            EnrollStudent(999, 1);                                         
            EnrollStudent(newStudentId, 999);                               
            EnrollStudent(newStudentId, 3);                                 

            
            Header("READ");
            FindCourseById(1);
            FindCourseById(999);
            FindCourseByTitle("Advanced EF Core");
            FindCourseByTitle("No Such Course");

            FilterCoursesByPrice(40m, 80m);
            FilterCoursesByPrice(90m, 10m); 
            PrintSubCategories(1);
            PrintSubCategories(999);
            PrintTopLevelCategories();

            PrintCourseRoster(1);
            PrintCourseRoster(999);

          
            Header("UPDATE");
            ChangePrice(newCourseId, 89.99m);
            ChangePrice(999, 10m);

            int ahmedId = StudentIdByEmail(AhmedEmail);
            UpdateProgress(ahmedId, 2, 150);          
            UpdateProgress(ahmedId, 2, 75);           
            UpdateProgress(ahmedId, 2, 100, 88.5m);   
            UpdateProgress(999, 999, 50);            

            PublishCourse(newCourseId);
            PublishCourse(999);
            EnrollStudent(newStudentId, newCourseId); 

            
            Header("DELETE");
            Unenroll(newStudentId, 3);
            Unenroll(newStudentId, 3); 

            DeleteCategoryUnsafe(1);   
            DeleteCategorySafe(1);     
            int tempCategoryId = AddTemporaryCategory("Temporary Category");
            DeleteCategorySafe(tempCategoryId); 

            DeleteCourseWithEnrollments(newCourseId);

            
            Header("BULK OPERATIONS");
            BulkRaisePrice(CourseLevel.Intermediate, 10);
            BulkDeleteDroppedEnrollments();
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("Unexpected error: " + (ex.InnerException?.Message ?? ex.Message));
            Console.WriteLine("Check that SQL Server / LocalDB is running and the connection string is correct.");
        }

        Console.WriteLine();
        Console.WriteLine("Done. Press any key to exit...");
        if (!Console.IsInputRedirected)
        {
            Console.ReadKey();
        }
    }



    private static void Header(string title)
    {
        Console.WriteLine();
        Console.WriteLine("==================== " + title + " ====================");
    }

    private static int StudentIdByEmail(string email)
    {
        using var db = new AppDbContext();
        return db.Students.Where(s => s.Email == email).Select(s => s.Id).FirstOrDefault();
    }

 

    private static void SeedStudentsAndEnrollments()
    {
        using var db = new AppDbContext();

        if (!db.Students.Any())
        {
            db.Students.AddRange(
                new Student { FullName = "Ahmed Samir", Email = AhmedEmail, DateOfBirth = new DateTime(2000, 5, 14) },
                new Student { FullName = "Mariam Ali", Email = MariamEmail, DateOfBirth = new DateTime(1999, 11, 2) },
                new Student { FullName = "Youssef Nabil", Email = YoussefEmail, DateOfBirth = new DateTime(2002, 7, 23) },
                new Student { FullName = "Hana Mahmoud", Email = "hana.mahmoud@example.com", DateOfBirth = new DateTime(2001, 1, 30) },
                new Student { FullName = "Karim Adel", Email = "karim.adel@example.com", DateOfBirth = new DateTime(1998, 9, 9) });
            db.SaveChanges();
            Console.WriteLine("Seeded 5 students.");
        }

        if (!db.Enrollments.Any())
        {
            var ids = db.Students.ToDictionary(s => s.Email, s => s.Id);

            Enrollment Make(string email, int courseId, string enrolledOn, int progress, EnrollmentStatus status, decimal? grade = null) =>
                new Enrollment
                {
                    StudentId = ids[email],
                    CourseId = courseId,
                    EnrolledOn = DateTime.Parse(enrolledOn),
                    ProgressPercentage = progress,
                    Status = status,
                    FinalGrade = grade
                };

            db.Enrollments.AddRange(
                Make(AhmedEmail, 1, "2024-02-01", 100, EnrollmentStatus.Completed, 92.5m),
                Make(AhmedEmail, 2, "2024-04-01", 40, EnrollmentStatus.Active),
                Make(MariamEmail, 1, "2024-02-10", 65, EnrollmentStatus.Active),
                Make(MariamEmail, 3, "2024-05-01", 100, EnrollmentStatus.Completed, 85m),
                Make(YoussefEmail, 1, "2024-03-01", 15, EnrollmentStatus.Dropped),
                Make(YoussefEmail, 4, "2024-06-01", 10, EnrollmentStatus.Active),
                Make("hana.mahmoud@example.com", 3, "2024-06-15", 5, EnrollmentStatus.Dropped),
                Make("karim.adel@example.com", 4, "2024-07-01", 100, EnrollmentStatus.Completed, 78m));
            db.SaveChanges();
            Console.WriteLine("Seeded 8 enrollments (Active, Completed and Dropped).");
        }
    }

    private static int AddInstructor(string fullName, string email, string? bio, DateTime joinedOn)
    {
        Console.WriteLine();
        Console.WriteLine($"-> Add instructor '{fullName}'");
        using var db = new AppDbContext();

        var existing = db.Instructors.FirstOrDefault(i => i.Email == email);
        if (existing != null)
        {
            Console.WriteLine($"   Instructor with email {email} already exists (Id = {existing.Id}).");
            return existing.Id;
        }

        var instructor = new Instructor { FullName = fullName, Email = email, Bio = bio, JoinedOn = joinedOn };
        db.Instructors.Add(instructor);
        db.SaveChanges();

        Console.WriteLine($"   Instructor inserted. Generated Id = {instructor.Id}");
        return instructor.Id;
    }

    private static int AddCourse(string title, string? description, decimal price, CourseLevel level, int instructorId, int categoryId)
    {
        Console.WriteLine();
        Console.WriteLine($"-> Add course '{title}' (instructor {instructorId}, category {categoryId})");
        using var db = new AppDbContext();

        if (!db.Instructors.Any(i => i.Id == instructorId))
        {
            Console.WriteLine($"   Refused: instructor with Id {instructorId} does not exist.");
            return 0;
        }

        if (!db.Categories.Any(c => c.Id == categoryId))
        {
            Console.WriteLine($"   Refused: category with Id {categoryId} does not exist.");
            return 0;
        }

        if (price < 0)
        {
            Console.WriteLine("   Refused: price cannot be negative.");
            return 0;
        }

        var existing = db.Courses.FirstOrDefault(c => c.Title == title);
        if (existing != null)
        {
            Console.WriteLine($"   A course with this title already exists (Id = {existing.Id}).");
            return existing.Id;
        }

        var course = new Course
        {
            Title = title,
            Description = description,
            Price = price,
            Level = level,
            IsPublished = false,
            PublishedOn = null,
            InstructorId = instructorId,   
            CategoryId = categoryId
        };

        db.Courses.Add(course);
        db.SaveChanges();

        Console.WriteLine($"   Course inserted. Generated Id = {course.Id}");
        return course.Id;
    }


    private static List<int> AddStudents()
    {
        Console.WriteLine();
        Console.WriteLine("-> Add three students (AddRange + one SaveChanges)");
        using var db = new AppDbContext();

        var students = new List<Student>
        {
            new Student { FullName = "Mona Adel", Email = "mona.adel@example.com", DateOfBirth = new DateTime(2001, 3, 12) },
            new Student { FullName = "Tarek Fouad", Email = "tarek.fouad@example.com", DateOfBirth = new DateTime(2000, 8, 19) },
            new Student { FullName = "Dina Wael", Email = "dina.wael@example.com", DateOfBirth = new DateTime(2003, 12, 5) }
        };
        var emails = students.Select(s => s.Email).ToList();

        if (db.Students.Any(s => emails.Contains(s.Email)))
        {
            Console.WriteLine("   These students already exist (unique email) - nothing inserted.");
        }
        else
        {
            db.Students.AddRange(students);
            db.SaveChanges();
            foreach (var s in students)
            {
                Console.WriteLine($"   {s.FullName} -> Id = {s.Id}");
            }
        }

        return db.Students.Where(s => emails.Contains(s.Email)).OrderBy(s => s.Id).Select(s => s.Id).ToList();
    }

    private static void EnrollStudent(int studentId, int courseId)
    {
        Console.WriteLine();
        Console.WriteLine($"-> Enroll student {studentId} in course {courseId}");
        using var db = new AppDbContext();

        var student = db.Students.Find(studentId);
        if (student == null)
        {
            Console.WriteLine($"   Refused: student with Id {studentId} does not exist.");
            return;
        }

        var course = db.Courses.Find(courseId);
        if (course == null)
        {
            Console.WriteLine($"   Refused: course with Id {courseId} does not exist.");
            return;
        }

        if (!course.IsPublished)
        {
            Console.WriteLine($"   Refused: course '{course.Title}' is not published yet.");
            return;
        }

        if (db.Enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId))
        {
            Console.WriteLine($"   Refused: {student.FullName} is already enrolled in '{course.Title}'.");
            return;
        }

        db.Enrollments.Add(new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrolledOn = DateTime.UtcNow,
            ProgressPercentage = 0,
            Status = EnrollmentStatus.Active,
            FinalGrade = null
        });
        db.SaveChanges();

        Console.WriteLine($"   {student.FullName} enrolled in '{course.Title}'.");
    }

    private static void FindCourseById(int id)
    {
        Console.WriteLine();
        Console.WriteLine($"-> Find course by Id ({id}) using Find");
        using var db = new AppDbContext();

        var course = db.Courses.Find(id);
        Console.WriteLine(course == null
            ? "   Course not found."
            : $"   Found: {course.Title} | {course.Level} | {course.Price:F2}");
    }

    private static void FindCourseByTitle(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"-> Find course by title ('{title}') using FirstOrDefault");
        using var db = new AppDbContext();

        var course = db.Courses.FirstOrDefault(c => c.Title == title);
        Console.WriteLine(course == null
            ? "   Course not found."
            : $"   Found: Id {course.Id} | {course.Level} | {course.Price:F2}");
    }

    private static void FilterCoursesByPrice(decimal min, decimal max)
    {
        Console.WriteLine();
        Console.WriteLine($"-> Courses priced between {min:F2} and {max:F2} (highest first)");

        if (min > max)
        {
            Console.WriteLine("   Invalid range: the minimum price is greater than the maximum price.");
            return;
        }

        using var db = new AppDbContext();

        var courses = db.Courses
            .Where(c => c.Price >= min && c.Price <= max)
            .OrderByDescending(c => c.Price)
            .Select(c => new { c.Title, c.Level, c.Price }) // only the 3 columns we print
            .ToList();

        if (courses.Count == 0)
        {
            Console.WriteLine("   No courses in this price range.");
            return;
        }

        foreach (var c in courses)
        {
            Console.WriteLine($"   {c.Title,-32} {c.Level,-13} {c.Price,8:F2}");
        }
    }

    private static void PrintSubCategories(int parentId)
    {
        Console.WriteLine();
        Console.WriteLine($"-> Direct sub-categories of category {parentId}");
        using var db = new AppDbContext();

        var parent = db.Categories.Find(parentId);
        if (parent == null)
        {
            Console.WriteLine($"   Category with Id {parentId} does not exist.");
            return;
        }

        var subs = db.Categories
            .Where(c => c.ParentCategoryId == parentId)
            .OrderBy(c => c.Name)
            .Select(c => c.Name)
            .ToList();

        if (subs.Count == 0)
        {
            Console.WriteLine($"   '{parent.Name}' has no sub-categories.");
            return;
        }

        Console.WriteLine($"   '{parent.Name}' -> {string.Join(", ", subs)}");
    }

    private static void PrintTopLevelCategories()
    {
        Console.WriteLine();
        Console.WriteLine("-> Top-level categories");
        using var db = new AppDbContext();

        var top = db.Categories
            .Where(c => c.ParentCategoryId == null)
            .OrderBy(c => c.Name)
            .Select(c => c.Name)
            .ToList();

        Console.WriteLine("   " + string.Join(", ", top));
    }

    private static void PrintCourseRoster(int courseId)
    {
        Console.WriteLine();
        Console.WriteLine($"-> Roster of course {courseId} (no joins)");
        using var db = new AppDbContext();

        var course = db.Courses.Find(courseId);
        if (course == null)
        {
            Console.WriteLine($"   Course with Id {courseId} does not exist.");
            return;
        }

        var studentIds = db.Enrollments
            .Where(e => e.CourseId == courseId)
            .Select(e => e.StudentId)
            .ToList();

        Console.WriteLine($"   '{course.Title}' has {studentIds.Count} enrolled student(s).");

        
        var names = db.Students
            .Where(s => studentIds.Contains(s.Id))
            .OrderBy(s => s.FullName)
            .Select(s => s.FullName)
            .ToList();

        foreach (var name in names)
        {
            Console.WriteLine($"   - {name}");
        }
    }

 

    private static void ChangePrice(int courseId, decimal newPrice)
    {
        Console.WriteLine();
        Console.WriteLine($"-> Change price of course {courseId} to {newPrice:F2}");

        if (newPrice < 0)
        {
            Console.WriteLine("   Rejected: price cannot be negative.");
            return;
        }

        using var db = new AppDbContext();

        var course = db.Courses.Find(courseId);
        if (course == null)
        {
            Console.WriteLine($"   Course with Id {courseId} does not exist.");
            return;
        }

        var oldPrice = course.Price;
        course.Price = newPrice;
        db.SaveChanges();

        Console.WriteLine($"   '{course.Title}': {oldPrice:F2} -> {newPrice:F2}");
    }

    private static void UpdateProgress(int studentId, int courseId, int progress, decimal? finalGrade = null)
    {
        Console.WriteLine();
        Console.WriteLine($"-> Update progress of student {studentId} in course {courseId} to {progress}%");

        if (progress < 0 || progress > 100)
        {
            Console.WriteLine("   Rejected: progress must be between 0 and 100.");
            return;
        }

        if (finalGrade is < 0 or > 100)
        {
            Console.WriteLine("   Rejected: final grade must be between 0 and 100.");
            return;
        }

        using var db = new AppDbContext();

        
        var enrollment = db.Enrollments.Find(studentId, courseId);
        if (enrollment == null)
        {
            Console.WriteLine("   Enrollment not found.");
            return;
        }

        if (enrollment.Status == EnrollmentStatus.Dropped)
        {
            Console.WriteLine("   Refused: this enrollment was dropped.");
            return;
        }

        enrollment.ProgressPercentage = progress;

        if (progress == 100)
        {
            enrollment.Status = EnrollmentStatus.Completed;
            if (finalGrade.HasValue)
            {
                enrollment.FinalGrade = finalGrade; 
            }
        }
        else if (enrollment.Status == EnrollmentStatus.Completed)
        {
           
            enrollment.Status = EnrollmentStatus.Active;
            enrollment.FinalGrade = null;
        }

        db.SaveChanges();

        Console.WriteLine($"   Progress = {enrollment.ProgressPercentage}%, status = {enrollment.Status}, grade = {(enrollment.FinalGrade?.ToString("F2") ?? "none")}");
    }

    private static void PublishCourse(int courseId)
    {
        Console.WriteLine();
        Console.WriteLine($"-> Publish course {courseId}");
        using var db = new AppDbContext();

        var course = db.Courses.Find(courseId);
        if (course == null)
        {
            Console.WriteLine($"   Course with Id {courseId} does not exist.");
            return;
        }

        if (course.IsPublished)
        {
            Console.WriteLine($"   '{course.Title}' is already published.");
            return;
        }

        course.IsPublished = true;
        course.PublishedOn = DateTime.UtcNow;
        db.SaveChanges();

        Console.WriteLine($"   '{course.Title}' published on {course.PublishedOn:yyyy-MM-dd HH:mm}.");
    }

    

    private static void Unenroll(int studentId, int courseId)
    {
        Console.WriteLine();
        Console.WriteLine($"-> Unenroll student {studentId} from course {courseId}");
        using var db = new AppDbContext();

        var enrollment = db.Enrollments.Find(studentId, courseId); 
        if (enrollment == null)
        {
            Console.WriteLine("   Enrollment not found - nothing to delete.");
            return;
        }

        db.Enrollments.Remove(enrollment);
        db.SaveChanges();

        Console.WriteLine("   Enrollment deleted.");
    }

   
    private static int AddTemporaryCategory(string name)
    {
        using var db = new AppDbContext();

        var existing = db.Categories.FirstOrDefault(c => c.Name == name);
        if (existing != null)
        {
            return existing.Id;
        }

        var category = new Category { Name = name, Description = "Created only to demonstrate a safe delete.", ParentCategoryId = null };
        db.Categories.Add(category);
        db.SaveChanges();
        return category.Id;
    }

    
    private static void DeleteCategoryUnsafe(int categoryId)
    {
        Console.WriteLine();
        Console.WriteLine($"-> UNSAFE delete of category {categoryId} (expecting a DbUpdateException)");
        using var db = new AppDbContext();

        var category = db.Categories.Find(categoryId);
        if (category == null)
        {
            Console.WriteLine($"   Category with Id {categoryId} does not exist.");
            return;
        }

        try
        {
            db.Categories.Remove(category);
            db.SaveChanges();
            Console.WriteLine("   Category deleted.");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("   DbUpdateException caught. Message:");
            Console.WriteLine("   " + (ex.InnerException?.Message ?? ex.Message));
        }
    }

    private static void DeleteCategorySafe(int categoryId)
    {
        Console.WriteLine();
        Console.WriteLine($"-> SAFE delete of category {categoryId}");
        using var db = new AppDbContext();

        var category = db.Categories.Find(categoryId);
        if (category == null)
        {
            Console.WriteLine($"   Category with Id {categoryId} does not exist.");
            return;
        }

        if (db.Categories.Any(c => c.ParentCategoryId == categoryId))
        {
            Console.WriteLine($"   Refused: '{category.Name}' still has sub-categories.");
            return;
        }

        if (db.Courses.Any(c => c.CategoryId == categoryId))
        {
            Console.WriteLine($"   Refused: '{category.Name}' still has courses.");
            return;
        }

        db.Categories.Remove(category);
        db.SaveChanges();
        Console.WriteLine($"   Category '{category.Name}' deleted.");
    }

    private static void DeleteCourseWithEnrollments(int courseId)
    {
        Console.WriteLine();
        Console.WriteLine($"-> Delete course {courseId} (it has enrollments)");
        using var db = new AppDbContext();

        var course = db.Courses.Find(courseId);
        if (course == null)
        {
            Console.WriteLine($"   Course with Id {courseId} does not exist.");
            return;
        }

        var before = db.Enrollments.Count(e => e.CourseId == courseId);
        Console.WriteLine($"   Enrollments before: {before}");

        db.Courses.Remove(course);
        db.SaveChanges();

        var after = db.Enrollments.Count(e => e.CourseId == courseId);
        Console.WriteLine($"   Enrollments after:  {after}");
    }

        

    private static void BulkRaisePrice(CourseLevel level, int percent)
    {
        Console.WriteLine();
        Console.WriteLine($"-> Raise price by {percent}% for all published {level} courses");
        using var db = new AppDbContext();

        decimal factor = 1m + percent / 100m;

        int affected = db.Courses
            .Where(c => c.IsPublished && c.Level == level)
            .ExecuteUpdate(s => s.SetProperty(c => c.Price, c => c.Price * factor));

        Console.WriteLine($"   Affected rows: {affected}");
    }

    private static void BulkDeleteDroppedEnrollments()
    {
        Console.WriteLine();
        Console.WriteLine("-> Delete all enrollments with status Dropped");
        using var db = new AppDbContext();

        int affected = db.Enrollments
            .Where(e => e.Status == EnrollmentStatus.Dropped)
            .ExecuteDelete();

        Console.WriteLine($"   Affected rows: {affected}");
    }
}
