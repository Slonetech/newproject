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

            // Seed Teacher Profile
            var teacherProfile = await context.Teachers.FirstOrDefaultAsync(t => t.UserId == teacherUser.Id);
            if (teacherProfile == null)
            {
                teacherProfile = new Teacher
                {
                    Id = Guid.NewGuid(),
                    UserId = teacherUser.Id,
                    FirstName = teacherUser.FirstName,
                    LastName = teacherUser.LastName,
                    Email = teacherUser.Email,
                    PhoneNumber = "123-456-7890",
                    Department = "Science",
                    HireDate = DateTime.UtcNow,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                context.Teachers.Add(teacherProfile);
            }

            // Seed Student Profile
            var studentProfile = await context.Students.FirstOrDefaultAsync(s => s.UserId == studentUser.Id);
            if (studentProfile == null)
            {
                studentProfile = new Student
                {
                    Id = Guid.NewGuid(),
                    UserId = studentUser.Id,
                    FirstName = studentUser.FirstName,
                    LastName = studentUser.LastName,
                    Email = studentUser.Email,
                    Grade = 10,
                    EnrollmentDate = DateTime.UtcNow,
                    IsActive = true
                };
                context.Students.Add(studentProfile);
            }

            // Seed Parent Profile
            var parentProfile = await context.Parents.FirstOrDefaultAsync(p => p.UserId == parentUser.Id);
            if (parentProfile == null)
            {
                parentProfile = new Parent
                {
                    Id = Guid.NewGuid(),
                    UserId = parentUser.Id,
                    FirstName = parentUser.FirstName,
                    LastName = parentUser.LastName,
                    Email = parentUser.Email,
                    PhoneNumber = "555-555-5555",
                    IsActive = true
                };
                context.Parents.Add(parentProfile);
            }

            await context.SaveChangesAsync();

            // Seed ParentChild relationship
            if (!await context.ParentChildren.AnyAsync(pc => pc.ParentId == parentProfile.Id && pc.StudentId == studentProfile.Id))
            {
                context.ParentChildren.Add(new ParentChild
                {
                    ParentId = parentProfile.Id,
                    StudentId = studentProfile.Id
                });
            }

            // Seed TeacherCourse (Assignment)
            var course = await context.Courses.FirstOrDefaultAsync();
            if (course != null && !await context.Set<TeacherCourse>().AnyAsync(tc => tc.TeacherId == teacherProfile.Id && tc.CourseId == course.Id))
            {
                context.Set<TeacherCourse>().Add(new TeacherCourse
                {
                    TeacherId = teacherProfile.Id,
                    CourseId = course.Id,
                    AssignmentDate = DateTime.UtcNow
                });
            }

            // Seed Enrollment
            if (course != null && !await context.Enrollments.AnyAsync(e => e.StudentId == studentProfile.Id && e.CourseId == course.Id))
            {
                context.Enrollments.Add(new Enrollment
                {
                    Id = Guid.NewGuid(),
                    StudentId = studentProfile.Id,
                    CourseId = course.Id,
                    EnrolledAt = DateTime.UtcNow
                });
            }

            await context.SaveChangesAsync();
        }
    }
}