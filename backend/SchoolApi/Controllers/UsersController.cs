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
                _logger.LogError(ex, "An error occurred while fetching users.");
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
                    return Conflict(new { Message = "Username already exists." });
                }

                // Check if email already exists
                var existingEmail = await _userManager.FindByEmailAsync(createUserDto.Email);
                if (existingEmail != null)
                {
                    return Conflict(new { Message = "Email already registered." });
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
                    _logger.LogError("User creation failed for {UserName}: {Errors}", createUserDto.UserName, string.Join(", ", errors));
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
                            await _userManager.DeleteAsync(user); // Clean up partially created user
                            return BadRequest(new { Message = $"Role '{role}' does not exist. User creation aborted." });
                        }
                    }
                }
                else
                {
                    // If no roles specified, assign default role (e.g., Student)
                    if (await _roleManager.RoleExistsAsync("Student"))
                    {
                        await _userManager.AddToRoleAsync(user, "Student");
                    }
                    else
                    {
                        _logger.LogWarning("Default 'Student' role does not exist. User created without a role.");
                    }
                }

                // Get the user's roles after assignment
                var roles = await _userManager.GetRolesAsync(user);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserDto
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
                _logger.LogError(ex, "An error occurred while creating the user.");
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
                return NotFound("User not found.");
            }

            // Check if updating email to an existing one (if changed)
            if (!string.Equals(user.Email, updateUserDto.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existingEmail = await _userManager.FindByEmailAsync(updateUserDto.Email);
                if (existingEmail != null && existingEmail.Id != user.Id)
                {
                    return Conflict(new { Message = "Email already registered to another user." });
                }
            }

            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.Email = updateUserDto.Email;
            user.UserName = updateUserDto.UserName; // Assuming you allow updating UserName

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError("User update failed for {UserId}: {Errors}", id, string.Join(", ", result.Errors.Select(e => e.Description)));
                return BadRequest(new { Message = "User update failed.", Errors = result.Errors.Select(e => e.Description) });
            }

            // Update roles if provided
            if (updateUserDto.Roles != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                var rolesToRemove = currentRoles.Except(updateUserDto.Roles).ToList();
                var rolesToAdd = updateUserDto.Roles.Except(currentRoles).ToList();

                if (rolesToRemove.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!removeResult.Succeeded)
                    {
                        _logger.LogError("Failed to remove roles for user {UserId}: {Errors}", id, string.Join(", ", removeResult.Errors.Select(e => e.Description)));
                        // Consider how to handle partial failures here. For now, we'll continue.
                    }
                }

                if (rolesToAdd.Any())
                {
                    var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!addResult.Succeeded)
                    {
                        _logger.LogError("Failed to add roles for user {UserId}: {Errors}", id, string.Join(", ", addResult.Errors.Select(e => e.Description)));
                        // Consider how to handle partial failures here.
                    }
                }
            }

            return NoContent();
        }

        // DELETE: api/Users/{id} (Soft Delete user and related profiles)
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

                // Handle related records based on roles
                if (roles.Contains("Teacher"))
                {
                    var teacher = await _context.Teachers
                                                .Include(t => t.TeacherCourses) // Eager load TeacherCourses
                                                .FirstOrDefaultAsync(t => t.UserId == id);
                    if (teacher != null)
                    {
                        // Remove all TeacherCourse associations for this teacher
                        _context.TeacherCourses.RemoveRange(teacher.TeacherCourses);
                        await _context.SaveChangesAsync(); // Save changes for associations first

                        // Now delete the teacher record (or soft delete it if applicable to your Teacher model)
                        _context.Teachers.Remove(teacher);
                    }
                }
                else if (roles.Contains("Student"))
                {
                    var student = await _context.Students
                                                .Include(s => s.StudentCourses)
                                                .Include(s => s.Grades)
                                                .Include(s => s.Attendances)
                                                .FirstOrDefaultAsync(s => s.UserId == id);
                    if (student != null)
                    {
                        // Remove student from all courses
                        _context.StudentCourses.RemoveRange(student.StudentCourses);

                        // Remove student's grades
                        _context.Grades.RemoveRange(student.Grades);

                        // Remove student's attendance records
                        _context.Attendances.RemoveRange(student.Attendances);

                        await _context.SaveChangesAsync(); // Save changes for related records first

                        // Now delete the student record (or soft delete it)
                        _context.Students.Remove(student);
                    }
                }
                else if (roles.Contains("Parent"))
                {
                    var parent = await _context.Parents
                                                .Include(p => p.Students) // Include students to nullify ParentId
                                                .FirstOrDefaultAsync(p => p.UserId == id);
                    if (parent != null)
                    {
                        // Update all students to remove the parent reference
                        foreach (var student in parent.Students)
                        {
                            student.ParentId = null; // Disassociate student from parent
                        }
                        await _context.SaveChangesAsync(); // Save changes for students first

                        // Now delete the parent record (or soft delete it)
                        _context.Parents.Remove(parent);
                    }
                }

                // Remove user from all roles (already done at the top, but leaving this for clarity
                // if you decide to structure it differently later)
                // await _userManager.RemoveFromRolesAsync(user, roles); // This was redundant here after line 208

                // Finally delete the user
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to delete user identity {UserId}: {Errors}", id, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return BadRequest(new { message = "Failed to delete user identity", errors = result.Errors.Select(e => e.Description) });
                }

                await _context.SaveChangesAsync(); // Final save for any pending profile deletions

                return Ok(new { message = "User and associated profiles deleted successfully" });
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
                return NotFound("User not found.");
            }

            // Validate if all roles in DTO exist
            foreach (var roleName in roleAssignmentDto.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    return BadRequest($"Role '{roleName}' does not exist.");
                }
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = roleAssignmentDto.Roles.Except(currentRoles).ToList();

            if (rolesToAdd.Any())
            {
                var result = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!result.Succeeded)
                {
                    return BadRequest(new { Message = "Failed to assign roles.", Errors = result.Errors.Select(e => e.Description) });
                }
            }
            // If you want to replace all roles, you'd remove currentRoles first, then add the new ones.
            // This method currently only adds new roles if they don't exist.

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
                return NotFound("User not found.");
            }

            // Validate if all roles in DTO exist (optional, but good practice)
            foreach (var roleName in roleAssignmentDto.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    _logger.LogWarning("Attempted to remove non-existent role '{RoleName}' from user {UserId}.", roleName, id);
                    // You might choose to return BadRequest or just ignore. Ignoring for now.
                }
            }

            var result = await _userManager.RemoveFromRolesAsync(user, roleAssignmentDto.Roles);
            if (!result.Succeeded)
            {
                return BadRequest(new { Message = "Failed to remove roles.", Errors = result.Errors.Select(e => e.Description) });
            }

            return NoContent();
        }

        // POST: api/Users/{userId}/link-student-profile
        // Note: This endpoint is for creating a new Student profile and linking it to an existing user.
        // If a student profile already exists (e.g., from an import), you'd need a different endpoint
        // to link an existing profile to an existing user.
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

            // You might want to get more details from the DTO instead of hardcoding "N/A" and current UTC
            var student = new Student
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FirstName = user.FirstName ?? string.Empty, // Use user's names
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty, // Use user's email
                DateOfBirth = DateTime.UtcNow, // Placeholder, ideally from DTO
                Address = "N/A", // Placeholder, ideally from DTO
                PhoneNumber = "N/A", // Placeholder, ideally from DTO
                EnrollmentDate = DateTime.UtcNow,
                IsActive = true // New property if you have soft delete for profiles
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

        // You might want to add similar endpoints for linking Teacher and Parent profiles
        // POST: api/Users/{userId}/link-teacher-profile
        [HttpPost("{userId}/link-teacher-profile")]
        public async Task<IActionResult> LinkTeacherProfile(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (await _context.Teachers.AnyAsync(t => t.UserId == userId))
            {
                return Conflict(new { Message = "User already linked to a teacher profile." });
            }

            var teacher = new Teacher
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                DateOfBirth = DateTime.UtcNow, // Placeholder
                Address = "N/A", // Placeholder
                PhoneNumber = "N/A", // Placeholder
                HireDate = DateTime.UtcNow,
                Specialization = "N/A", // Placeholder, ideally from DTO
                Department = "N/A", // Placeholder, ideally from DTO
                IsActive = true // New property if you have soft delete for profiles
            };
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            if (!await _userManager.IsInRoleAsync(user, "Teacher"))
            {
                await _userManager.AddToRoleAsync(user, "Teacher");
            }

            return Ok(new { Message = "Teacher profile linked successfully." });
        }

        // POST: api/Users/{userId}/link-parent-profile
        [HttpPost("{userId}/link-parent-profile")]
        public async Task<IActionResult> LinkParentProfile(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (await _context.Parents.AnyAsync(p => p.UserId == userId))
            {
                return Conflict(new { Message = "User already linked to a parent profile." });
            }

            var parent = new Parent
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = "N/A", // Placeholder
                Address = "N/A", // Placeholder
                IsActive = true // New property if you have soft delete for profiles
            };
            _context.Parents.Add(parent);
            await _context.SaveChangesAsync();

            if (!await _userManager.IsInRoleAsync(user, "Parent"))
            {
                await _userManager.AddToRoleAsync(user, "Parent");
            }

            return Ok(new { Message = "Parent profile linked successfully." });
        }
    }
}