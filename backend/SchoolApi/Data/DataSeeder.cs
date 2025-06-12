using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolApi.Data
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DataSeeder(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedDataAsync()
        {
            // Create roles if they don't exist
            await CreateRolesAsync();

            // Create admin user
            await CreateAdminUserAsync();

            // Create teachers
            await CreateTeachersAsync();

            // Create students
            await CreateStudentsAsync();

            // Create parents
            await CreateParentsAsync();

            // Create courses
            await CreateCoursesAsync();

            // Assign courses to teachers and students
            await AssignCoursesAsync();
        }

        private async Task CreateRolesAsync()
        {
            string[] roles = { "Admin", "Teacher", "Student", "Parent" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private async Task CreateAdminUserAsync()
        {
            var adminEmail = "admin@school.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        private async Task CreateTeachersAsync()
        {
            var teachers = new List<(string Email, string FirstName, string LastName, string Department)>
            {
                ("john.doe@school.com", "John", "Doe", "Mathematics"),
                ("jane.smith@school.com", "Jane", "Smith", "Science"),
                ("mike.johnson@school.com", "Mike", "Johnson", "English")
            };

            foreach (var teacher in teachers)
            {
                var user = await _userManager.FindByEmailAsync(teacher.Email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = teacher.Email,
                        Email = teacher.Email,
                        FirstName = teacher.FirstName,
                        LastName = teacher.LastName,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user, "Teacher123!");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Teacher");

                        var teacherEntity = new Teacher
                        {
                            UserId = user.Id,
                            FirstName = teacher.FirstName,
                            LastName = teacher.LastName,
                            Email = teacher.Email,
                            Department = teacher.Department,
                            PhoneNumber = "123-456-7890"
                        };

                        _context.Teachers.Add(teacherEntity);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task CreateStudentsAsync()
        {
            var students = new List<(string Email, string FirstName, string LastName, int Grade)>
            {
                ("alice.brown@school.com", "Alice", "Brown", 9),
                ("bob.wilson@school.com", "Bob", "Wilson", 10),
                ("carol.davis@school.com", "Carol", "Davis", 11)
            };

            foreach (var student in students)
            {
                var user = await _userManager.FindByEmailAsync(student.Email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = student.Email,
                        Email = student.Email,
                        FirstName = student.FirstName,
                        LastName = student.LastName,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user, "Student123!");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Student");

                        var studentEntity = new Student
                        {
                            UserId = user.Id,
                            FirstName = student.FirstName,
                            LastName = student.LastName,
                            Email = student.Email,
                            Grade = student.Grade,
                            PhoneNumber = "123-456-7890",
                            DateOfBirth = DateTime.Now.AddYears(-student.Grade - 5),
                            Address = "123 School St"
                        };

                        _context.Students.Add(studentEntity);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task CreateParentsAsync()
        {
            var parents = new List<(string Email, string FirstName, string LastName)>
            {
                ("parent1@school.com", "Robert", "Brown"),
                ("parent2@school.com", "Susan", "Wilson"),
                ("parent3@school.com", "David", "Davis")
            };

            foreach (var parent in parents)
            {
                var user = await _userManager.FindByEmailAsync(parent.Email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = parent.Email,
                        Email = parent.Email,
                        FirstName = parent.FirstName,
                        LastName = parent.LastName,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user, "Parent123!");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Parent");

                        var parentEntity = new Parent
                        {
                            UserId = user.Id,
                            FirstName = parent.FirstName,
                            LastName = parent.LastName,
                            Email = parent.Email,
                            PhoneNumber = "123-456-7890",
                            Address = "123 School St"
                        };

                        _context.Parents.Add(parentEntity);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task CreateCoursesAsync()
        {
            var courses = new List<(string Title, string Description, int Credits)>
            {
                ("Mathematics 101", "Introduction to Algebra", 3),
                ("Science 101", "Basic Physics", 3),
                ("English 101", "Composition and Literature", 3)
            };

            foreach (var course in courses)
            {
                if (!await _context.Courses.AnyAsync(c => c.Title == course.Title))
                {
                    var courseEntity = new Course
                    {
                        Title = course.Title,
                        Description = course.Description,
                        Credits = course.Credits
                    };

                    _context.Courses.Add(courseEntity);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task AssignCoursesAsync()
        {
            var teachers = await _context.Teachers.ToListAsync();
            var courses = await _context.Courses.ToListAsync();
            var students = await _context.Students.ToListAsync();

            // Assign courses to teachers
            for (int i = 0; i < teachers.Count && i < courses.Count; i++)
            {
                courses[i].TeacherId = teachers[i].Id;
            }

            // Assign courses to students
            foreach (var student in students)
            {
                foreach (var course in courses)
                {
                    if (!await _context.StudentCourses.AnyAsync(sc =>
                        sc.StudentId == student.Id && sc.CourseId == course.Id))
                    {
                        _context.StudentCourses.Add(new StudentCourse
                        {
                            StudentId = student.Id,
                            CourseId = course.Id
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}