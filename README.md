# Online Learning Platform - EF Core Session 03

Console app, EF Core 8 (Code-First) + SQL Server.

## Run
1. Install the .NET 8 SDK and SQL Server (LocalDB is enough).
2. Default connection string (in `Data/AppDbContext.cs`):
   `Server=(localdb)\MSSQLLocalDB;Database=OnlineLearningPlatformDb;Trusted_Connection=True;TrustServerCertificate=True`
   Use another server by setting the `OLP_CONNECTION` environment variable.
3. `dotnet run`
   The app applies the migrations (`Database.Migrate()`), seeds students/enrollments only if empty, then runs every operation in order.

Or apply the migrations manually first: `dotnet ef database update`

## Structure
- `Entities/`, `Enums/` - model
- `Data/Configurations/` - one `IEntityTypeConfiguration<T>` per entity (Fluent API + HasData)
- `Migrations/` - `InitialCreate` (model) and `SeedData` (HasData)
- `Program.cs` - code seeding + one method per operation

## Regenerating migrations (optional)
If you prefer EF-generated migration files:
```
rm -r Migrations
dotnet ef migrations add InitialCreate   # with HasData temporarily commented out
dotnet ef migrations add SeedData        # with HasData restored
dotnet ef database update
```
