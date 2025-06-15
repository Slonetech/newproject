using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.Models;
using SchoolApi.Models.DTOs.Users;
using Microsoft.Extensions.Logging;

namespace SchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Only users with the 'Admin' role can access this controller
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context; // To manage Student/Teacher/Parent profiles
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserManager<ApplicationUser> userManager,
                               RoleManager<IdentityRole> roleManager,
                               ApplicationDbContext context,
                               ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
        }

        // GET: api/Users
        // Get all users with their roles and linked profiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            try
            {
                // Get all users first
                var users = await _userManager.Users.ToListAsync();
                var userDtos = new List<UserDto>();

                // Process each user's roles separately
                foreach (var user in users)
                {
                    // Get roles for the current user
                    var roles = await _userManager.GetRolesAsync(user);

                    // Create DTO for the current user
                    var userDto = new UserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Roles = roles.ToList()
                    };

                    userDtos.Add(userDto);
                }

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { Message = "An error occurred while fetching users.", Error = ex.Message });
            }
        }

        // GET: api/Users/{id}
        // Get a specific user by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList()
            };
        }

        // POST: api/Users
        // Create a new ApplicationUser with an initial role and optional profile
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new { Message = "Validation failed", Errors = errors });
                }

                // Check if username already exists
                var existingUser = await _userManager.FindByNameAsync(createUserDto.UserName);
                if (existingUser != null)
                {
                    return BadRequest(new { Message = "Username already exists." });
                }

                // Check if email already exists
                var existingEmail = await _userManager.FindByEmailAsync(createUserDto.Email);
                if (existingEmail != null)
                {
                    return BadRequest(new { Message = "Email already registered." });
                }

                // Validate password
                if (string.IsNullOrEmpty(createUserDto.Password))
                {
                    return BadRequest(new { Message = "Password is required." });
                }

                var user = new ApplicationUser
                {
                    UserName = createUserDto.UserName,
                    Email = createUserDto.Email,
                    FirstName = createUserDto.FirstName,
                    LastName = createUserDto.LastName
                };

                var result = await _userManager.CreateAsync(user, createUserDto.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return BadRequest(new { Message = "User creation failed.", Errors = errors });
                }

                // Assign roles
                if (createUserDto.Roles != null && createUserDto.Roles.Any())
                {
                    foreach (var role in createUserDto.Roles)
                    {
                        if (await _roleManager.RoleExistsAsync(role))
                        {
                            await _userManager.AddToRoleAsync(user, role);
                        }
                        else
                        {
                            // If role doesn't exist, remove the user and return error
                            await _userManager.DeleteAsync(user);
                            return BadRequest(new { Message = $"Role '{role}' does not exist." });
                        }
                    }
                }
                else
                {
                    // If no roles specified, assign default role
                    if (await _roleManager.RoleExistsAsync("Student"))
                    {
                        await _userManager.AddToRoleAsync(user, "Student");
                    }
                }

                // Get the user's roles after assignment
                var roles = await _userManager.GetRolesAsync(user);
                return Ok(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles.ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the user.", Error = ex.Message });
            }
        }

        // PUT: api/Users/{id}
        // Update user details
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.Email = updateUserDto.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Update roles
            if (updateUserDto.Roles != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRolesAsync(user, updateUserDto.Roles);
            }

            return NoContent();
        }

        // DELETE: api/Users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Get user's roles
                var roles = await _userManager.GetRolesAsync(user);

                // Remove user from all roles first
                await _userManager.RemoveFromRolesAsync(user, roles);

                // Handle related records based on roles
                if (roles.Contains("Teacher"))
                {
                    var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == id);
                    if (teacher != null)
                    {
                        // Update all courses to remove the teacher reference
                        var courses = await _context.Courses
                            .Where(c => c.TeacherId == teacher.Id)
                            .ToListAsync();

                        foreach (var course in courses)
                        {
                            course.TeacherId = null;
                        }
                        await _context.SaveChangesAsync();

                        // Now delete the teacher record
                        _context.Teachers.Remove(teacher);
                    }
                }
                else if (roles.Contains("Student"))
                {
                    var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == id);
                    if (student != null)
                    {
                        // Remove student from all courses
                        var studentCourses = await _context.StudentCourses
                            .Where(sc => sc.StudentId == student.Id)
                            .ToListAsync();
                        _context.StudentCourses.RemoveRange(studentCourses);

                        // Remove student's grades
                        var grades = await _context.Grades
                            .Where(g => g.StudentId == student.Id)
                            .ToListAsync();
                        _context.Grades.RemoveRange(grades);

                        // Remove student's attendance records
                        var attendances = await _context.Attendances
                            .Where(a => a.StudentId == student.Id)
                            .ToListAsync();
                        _context.Attendances.RemoveRange(attendances);

                        await _context.SaveChangesAsync();

                        // Now delete the student record
                        _context.Students.Remove(student);
                    }
                }
                else if (roles.Contains("Parent"))
                {
                    var parent = await _context.Parents.FirstOrDefaultAsync(p => p.UserId == id);
                    if (parent != null)
                    {
                        // Update all students to remove the parent reference
                        var students = await _context.Students
                            .Where(s => s.ParentId == parent.Id)
                            .ToListAsync();

                        foreach (var student in students)
                        {
                            student.ParentId = null;
                        }
                        await _context.SaveChangesAsync();

                        // Now delete the parent record
                        _context.Parents.Remove(parent);
                    }
                }

                // Save changes for related records
                await _context.SaveChangesAsync();

                // Finally delete the user
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "Failed to delete user", errors = result.Errors });
                }

                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the user", error = ex.Message });
            }
        }

        // POST: api/Users/{id}/roles
        // Assign roles to a user
        [HttpPost("{id}/roles")]
        public async Task<IActionResult> AssignRoles(string id, RoleAssignmentDto roleAssignmentDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRolesAsync(user, roleAssignmentDto.Roles);

            return NoContent();
        }

        // DELETE: api/Users/{id}/roles
        // Remove roles from a user
        [HttpDelete("{id}/roles")]
        public async Task<IActionResult> RemoveRoles(string id, RoleAssignmentDto roleAssignmentDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.RemoveFromRolesAsync(user, roleAssignmentDto.Roles);

            return NoContent();
        }

        // POST: api/Users/{userId}/link-student-profile
        [HttpPost("{userId}/link-student-profile")]
        public async Task<IActionResult> LinkStudentProfile(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (await _context.Students.AnyAsync(s => s.UserId == userId))
            {
                return Conflict(new { Message = "User already linked to a student profile." });
            }

            var student = new Student
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                DateOfBirth = DateTime.UtcNow,
                Address = "N/A",
                PhoneNumber = "N/A",
                EnrollmentDate = DateTime.UtcNow
            };
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            // Ensure the user has the Student role
            if (!await _userManager.IsInRoleAsync(user, "Student"))
            {
                await _userManager.AddToRoleAsync(user, "Student");
            }

            return Ok(new { Message = "Student profile linked successfully." });
        }
    }
}
