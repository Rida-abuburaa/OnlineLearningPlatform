using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearningPlatform.Entities;

namespace OnlineLearningPlatform.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Description).HasMaxLength(500); // optional

        // Self-referencing one-to-many: a category has many sub-categories, each with exactly one parent.
        // Restrict => a parent that still has sub-categories cannot be deleted.
        builder.HasOne(c => c.ParentCategory)
               .WithMany(c => c.SubCategories)
               .HasForeignKey(c => c.ParentCategoryId)
               .OnDelete(DeleteBehavior.Restrict);

        // Three levels: Programming -> Backend -> ASP.NET   and   Design -> UI/UX Design
        /*builder.HasData(
            new Category { Id = 1, Name = "Programming", Description = "Software development courses", ParentCategoryId = null },
            new Category { Id = 2, Name = "Backend", Description = "Server-side development", ParentCategoryId = 1 },
            new Category { Id = 3, Name = "ASP.NET", Description = "Web development with ASP.NET Core", ParentCategoryId = 2 },
            new Category { Id = 4, Name = "Frontend", Description = "Client-side development", ParentCategoryId = 1 },
            new Category { Id = 5, Name = "Design", Description = null, ParentCategoryId = null },
            new Category { Id = 6, Name = "UI/UX Design", Description = "User interface and experience design", ParentCategoryId = 5 });*/
    }
}
