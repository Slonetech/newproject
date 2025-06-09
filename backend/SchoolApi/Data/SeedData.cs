using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Models; // Assuming ApplicationUser is here

namespace SchoolApi.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed Roles
            string[] roleNames = { "Admin", "Teacher", "Student", "Parent" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed Admin User
            string adminUserName = "admin@school.com";
            string adminPassword = "Admin@123"; // CHANGE THIS! In production, ensure this is a strong, unique password.

            if (await userManager.FindByNameAsync(adminUserName) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminUserName,
                    Email = adminUserName,
                    EmailConfirmed = true,
                    FirstName = "Super",
                    LastName = "Admin"
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    // Log errors if admin user creation fails
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error creating admin user: {error.Description}");
                    }
                }
            }

            // You can add more seed data here for teachers, students, courses, etc.
            // Example for seeding a teacher:
            // if (await userManager.FindByNameAsync("teacher@school.com") == null)
            // {
            //     var teacherUser = new ApplicationUser { UserName = "teacher@school.com", Email = "teacher@school.com", EmailConfirmed = true, FirstName = "Jane", LastName = "Doe" };
            //     var teacherResult = await userManager.CreateAsync(teacherUser, "Teacher@123");
            //     if (teacherResult.Succeeded)
            //     {
            //         await userManager.AddToRoleAsync(teacherUser, "Teacher");
            //         // Link to Teacher profile
            //         var teacherProfile = new Teacher {
            //             FirstName = "Jane", LastName = "Doe", Email = "teacher@school.com",
            //             PhoneNumber = "123-456-7890", Department = "Science", UserId = teacherUser.Id
            //         };
            //         var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            //         dbContext.Teachers.Add(teacherProfile);
            //         await dbContext.SaveChangesAsync();
            //     }
            // }

            // Similarly for students and parents.
        }
    }
}
