using _40PlusRipped.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _40PlusRipped.Data
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<FortyPlusRippedDbContext>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // Ensure database is created and migrations are applied
                await context.Database.MigrateAsync();

                // Seed data
                await SeedRolesAsync(roleManager);
                await SeedUsersAsync(userManager);
                await SeedCoreAreasAsync(context);

                // Add more seed methods as needed
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
                throw;
            }
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            // Check if roles already exist
            if (await roleManager.Roles.AnyAsync())
                return;

            // Define roles
            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            // Check if admin user exists
            if (await userManager.Users.AnyAsync())
                return;

            // Create admin user
            var adminUser = new ApplicationUser
            {
                UserName = "admin@40plusripped.com",
                Email = "admin@40plusripped.com",
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                DateOfBirth = new DateTime(1980, 1, 1),
                Gender = "Not specified"
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Create a test user
            var testUser = new ApplicationUser
            {
                UserName = "user@40plusripped.com",
                Email = "user@40plusripped.com",
                EmailConfirmed = true,
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = new DateTime(1975, 6, 15),
                Gender = "Male"
            };

            result = await userManager.CreateAsync(testUser, "User123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(testUser, "User");
            }
        }

        private static async Task SeedCoreAreasAsync(FortyPlusRippedDbContext context)
        {
            // Check if core areas exist
            if (await context.CoreAreas.AnyAsync())
                return;

            // Define core areas
            var coreAreas = new List<CoreArea>
            {
                new CoreArea
                {
                    Name = "Sleep",
                    Description = "Focus on improving sleep quality and duration.",
                    IconUrl = "/assets/icons/sleep.png"
                },
                new CoreArea
                {
                    Name = "CalorieConsumption",
                    Description = "Manage your calorie intake effectively.",
                    IconUrl = "/assets/icons/calories.png"
                },
                new CoreArea
                {
                    Name = "ProteinConsumption",
                    Description = "Ensure adequate protein intake for muscle maintenance.",
                    IconUrl = "/assets/icons/protein.png"
                },
                new CoreArea
                {
                    Name = "Exercise",
                    Description = "Adaptive and progressive exercise routines.",
                    IconUrl = "/assets/icons/exercise.png"
                }
            };

            context.CoreAreas.AddRange(coreAreas);
            await context.SaveChangesAsync();
        }
    }
}