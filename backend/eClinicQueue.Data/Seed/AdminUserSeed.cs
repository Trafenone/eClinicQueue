using eClinicQueue.Data.Models;
using eClinicQueue.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eClinicQueue.Data.Seed;

public static class AdminUserSeed
{
    public static async Task SeedAdminUser(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                FirstName = "System",
                LastName = "Administrator",
                PhoneNumber = string.Empty,
                RoleId = 2,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            if (await context.Users.AnyAsync(u => u.Email == adminUser.Email))
            {
                return;
            }

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();

            logger.LogInformation("Admin user has been seeded successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the admin user");
        }
    }
}