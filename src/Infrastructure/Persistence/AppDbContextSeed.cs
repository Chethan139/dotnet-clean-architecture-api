using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public static class AppDbContextSeed
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        try
        {
            await context.Database.MigrateAsync();

            if (!await context.Tasks.AnyAsync())
            {
                var seedTasks = new[]
                {
                    TaskItem.Create(
                        "Set up CI/CD pipeline",
                        "Configure GitHub Actions for automated build, test, and deployment.",
                        TaskPriority.High,
                        DateTime.UtcNow.AddDays(7),
                        "devops@company.com",
                        "admin"),

                    TaskItem.Create(
                        "Implement authentication module",
                        "Add JWT-based authentication with refresh token support.",
                        TaskPriority.Critical,
                        DateTime.UtcNow.AddDays(3),
                        "backend@company.com",
                        "admin"),

                    TaskItem.Create(
                        "Write API documentation",
                        "Document all API endpoints using Swagger/OpenAPI specification.",
                        TaskPriority.Medium,
                        DateTime.UtcNow.AddDays(14),
                        "backend@company.com",
                        "admin"),

                    TaskItem.Create(
                        "Performance testing",
                        "Run load tests against all critical endpoints and optimize bottlenecks.",
                        TaskPriority.Low,
                        DateTime.UtcNow.AddDays(21),
                        "qa@company.com",
                        "admin")
                };

                await context.Tasks.AddRangeAsync(seedTasks);
                await context.SaveChangesAsync();

                logger.LogInformation("Database seeded successfully with {Count} tasks.", seedTasks.Length);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}
