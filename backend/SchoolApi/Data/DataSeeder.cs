using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolApi.Data
{
    public static class DataSeeder
    {
        public static async Task SeedData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed Roles
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("Teacher"))
            {
                await roleManager.CreateAsync(new IdentityRole("Teacher"));
            }
            if (!await roleManager.RoleExistsAsync("Student"))
            {
                await roleManager.CreateAsync(new IdentityRole("Student"));
            }
            if (!await roleManager.RoleExistsAsync("Parent"))
            {
                await roleManager.CreateAsync(new IdentityRole("Parent"));
            }

            // Seed Admin User
            var adminEmail = "admin@school.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true,
                    SecurityStamp = "00000000-0000-0000-0000-000000000000" // Static GUID
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed Sample Teacher
            var teacherEmail = "teacher@school.com";
            var teacherUser = await userManager.FindByEmailAsync(teacherEmail);

            if (teacherUser == null)
            {
                teacherUser = new ApplicationUser
                {
                    UserName = teacherEmail,
                    Email = teacherEmail,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailConfirmed = true,
                    SecurityStamp = "11111111-1111-1111-1111-111111111111" // Static GUID
                };

                var result = await userManager.CreateAsync(teacherUser, "Teacher123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(teacherUser, "Teacher");
                }
            }

            // Seed Sample Student
            var studentEmail = "student@school.com";
            var studentUser = await userManager.FindByEmailAsync(studentEmail);

            if (studentUser == null)
            {
                studentUser = new ApplicationUser
                {
                    UserName = studentEmail,
                    Email = studentEmail,
                    FirstName = "Jane",
                    LastName = "Smith",
                    EmailConfirmed = true,
                    SecurityStamp = "22222222-2222-2222-2222-222222222222" // Static GUID
                };

                var result = await userManager.CreateAsync(studentUser, "Student123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(studentUser, "Student");
                }
            }

            // Seed Sample Parent
            var parentEmail = "parent@school.com";
            var parentUser = await userManager.FindByEmailAsync(parentEmail);

            if (parentUser == null)
            {
                parentUser = new ApplicationUser
                {
                    UserName = parentEmail,
                    Email = parentEmail,
                    FirstName = "Robert",
                    LastName = "Johnson",
                    EmailConfirmed = true,
                    SecurityStamp = "33333333-3333-3333-3333-333333333333" // Static GUID
                };

                var result = await userManager.CreateAsync(parentUser, "Parent123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(parentUser, "Parent");
                }
            }

            // Save changes
            await context.SaveChangesAsync();
        }
    }
}