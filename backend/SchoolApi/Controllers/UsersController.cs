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
using SchoolApi.Models.DTOs.Admin; // Import new DTOs

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

        public UsersController(UserManager<ApplicationUser> userManager,
                               RoleManager<IdentityRole> roleManager,
                               ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: api/Users
        // Get all users with their roles and linked profiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName ?? string.Empty, // Handle potential null from ApplicationUser
                    Email = user.Email ?? string.Empty,       // Handle potential null from ApplicationUser
                    FirstName = user.FirstName ?? string.Empty, // Handle potential null from ApplicationUser
                    LastName = user.LastName ?? string.Empty,   // Handle potential null from ApplicationUser
                    Roles = roles.ToList()
                };

                // Try to link to specific profiles if they exist
                var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
                if (student != null) userDto.StudentId = student.StudentId;

                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);
                if (teacher != null) userDto.TeacherId = teacher.TeacherId;

                var parent = await _context.Parents.FirstOrDefaultAsync(p => p.UserId == user.Id);
                if (parent != null) userDto.ParentId = parent.ParentId;

                userDtos.Add(userDto);
            }

            return Ok(userDtos);
        }

        // GET: api/Users/{id}
        // Get a specific user by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Roles = roles.ToList()
            };

            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (student != null) userDto.StudentId = student.StudentId;

            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);
            if (teacher != null) userDto.TeacherId = teacher.TeacherId;

            var parent = await _context.Parents.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (parent != null) userDto.ParentId = parent.ParentId;

            return Ok(userDto);
        }

        // POST: api/Users
        // Create a new ApplicationUser with an initial role and optional profile
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreationDto userCreationDto)
        {
            if (!ModelState.IsValid)
            {
                // Return specific validation errors from ModelState
                return BadRequest(ModelState);
            }

            // [Required] attributes on UserCreationDto properties handle these checks
            // So, explicit null checks here are redundant if ModelState.IsValid is false.
            // However, the compiler might still warn if not explicitly handled here.
            // For robustness, ensure UserCreationDto fields are not null from the client.

            var userExists = await _userManager.FindByNameAsync(userCreationDto.Username);
            if (userExists != null)
            {
                return BadRequest(new { Message = "Username already exists." });
            }

            var emailExists = await _userManager.FindByEmailAsync(userCreationDto.Email);
            if (emailExists != null)
            {
                return BadRequest(new { Message = "Email already registered." });
            }

            var newUser = new ApplicationUser
            {
                UserName = userCreationDto.Username,
                Email = userCreationDto.Email,
                EmailConfirmed = true,
                FirstName = userCreationDto.FirstName,
                LastName = userCreationDto.LastName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var createResult = await _userManager.CreateAsync(newUser, userCreationDto.Password);
            if (!createResult.Succeeded)
            {
                return BadRequest(new { Message = "User creation failed.", Errors = createResult.Errors });
            }

            // Assign initial role
            // userCreationDto.InitialRole is [Required] string, so it should not be null
            if (!await _roleManager.RoleExistsAsync(userCreationDto.InitialRole))
            {
                return BadRequest(new { Message = $"Role '{userCreationDto.InitialRole}' does not exist." });
            }
            await _userManager.AddToRoleAsync(newUser, userCreationDto.InitialRole);

            // OPTION: Automatically create a profile based on the initial role
            // newUser.FirstName, newUser.LastName, newUser.Email are assigned from userCreationDto,
            // which has them as [Required] string. So they should not be null.
            await CreateUserProfile(newUser, userCreationDto.InitialRole, newUser.FirstName, newUser.LastName, newUser.Email);

            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, new { Message = "User created successfully with role." });
        }

        // PUT: api/Users/{id}
        // Update user details
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDto userUpdateDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (!string.IsNullOrEmpty(userUpdateDto.Username) && userUpdateDto.Username != user.UserName)
            {
                // userUpdateDto.Username is nullable, but we check for null/empty.
                // FindByNameAsync expects non-null string, so use null-forgiving operator if sure.
                var userByUsername = await _userManager.FindByNameAsync(userUpdateDto.Username!);
                if (userByUsername != null && userByUsername.Id != id)
                {
                    return BadRequest(new { Message = "Username is already taken." });
                }
                user.UserName = userUpdateDto.Username;
                user.NormalizedUserName = _userManager.NormalizeName(userUpdateDto.Username);
            }

            if (!string.IsNullOrEmpty(userUpdateDto.Email) && userUpdateDto.Email != user.Email)
            {
                // userUpdateDto.Email is nullable, but we check for null/empty.
                var userByEmail = await _userManager.FindByEmailAsync(userUpdateDto.Email!);
                if (userByEmail != null && userByEmail.Id != id)
                {
                    return BadRequest(new { Message = "Email is already taken." });
                }
                user.Email = userUpdateDto.Email;
                user.NormalizedEmail = _userManager.NormalizeEmail(userUpdateDto.Email);
            }

            if (!string.IsNullOrEmpty(userUpdateDto.FirstName))
            {
                user.FirstName = userUpdateDto.FirstName;
            }

            if (!string.IsNullOrEmpty(userUpdateDto.LastName))
            {
                user.LastName = userUpdateDto.LastName;
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest(new { Message = "User update failed.", Errors = updateResult.Errors });
            }

            return NoContent();
        }

        // DELETE: api/Users/{id}
        // Delete a user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var studentProfile = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (studentProfile != null) _context.Students.Remove(studentProfile);

            var teacherProfile = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);
            if (teacherProfile != null) _context.Teachers.Remove(teacherProfile);

            var parentProfile = await _context.Parents.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (parentProfile != null) _context.Parents.Remove(parentProfile);

            await _context.SaveChangesAsync();

            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return BadRequest(new { Message = "User deletion failed.", Errors = deleteResult.Errors });
            }

            return NoContent();
        }

        // POST: api/Users/{userId}/assign-role
        // Assign a role to a user
        [HttpPost("{userId}/assign-role")]
        public async Task<IActionResult> AssignRole(string userId, [FromBody] RoleAssignmentDto roleAssignmentDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // roleAssignmentDto.RoleName is [Required] string
            if (string.IsNullOrEmpty(roleAssignmentDto.RoleName))
            {
                 return BadRequest(new { Message = "Role name must be provided." });
            }

            if (!await _roleManager.RoleExistsAsync(roleAssignmentDto.RoleName))
            {
                return BadRequest(new { Message = $"Role '{roleAssignmentDto.RoleName}' does not exist." });
            }

            var result = await _userManager.AddToRoleAsync(user, roleAssignmentDto.RoleName);
            if (!result.Succeeded)
            {
                return BadRequest(new { Message = "Failed to assign role.", Errors = result.Errors });
            }

            // user.FirstName etc. are nullable from ApplicationUser
            await CreateUserProfile(user, roleAssignmentDto.RoleName, user.FirstName, user.LastName, user.Email);

            return Ok(new { Message = $"Role '{roleAssignmentDto.RoleName}' assigned successfully." });
        }

        // POST: api/Users/{userId}/remove-role
        // Remove a role from a user
        [HttpPost("{userId}/remove-role")]
        public async Task<IActionResult> RemoveRole(string userId, [FromBody] RoleAssignmentDto roleAssignmentDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // roleAssignmentDto.RoleName is [Required] string
            if (string.IsNullOrEmpty(roleAssignmentDto.RoleName))
            {
                 return BadRequest(new { Message = "Role name must be provided." });
            }

            if (!await _roleManager.RoleExistsAsync(roleAssignmentDto.RoleName))
            {
                return BadRequest(new { Message = $"Role '{roleAssignmentDto.RoleName}' does not exist." });
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleAssignmentDto.RoleName);
            if (!result.Succeeded)
            {
                return BadRequest(new { Message = "Failed to remove role.", Errors = result.Errors });
            }

            return Ok(new { Message = $"Role '{roleAssignmentDto.RoleName}' removed successfully." });
        }

        // Helper method to create a linked profile based on role
        private async Task CreateUserProfile(ApplicationUser user, string role, string? firstName, string? lastName, string? email)
        {
            // Use null-coalescing operator to provide empty string defaults for nullable inputs
            // to satisfy the 'required' properties of the profile models.
            string profileFirstName = firstName ?? string.Empty;
            string profileLastName = lastName ?? string.Empty;
            string profileEmail = email ?? string.Empty;

            switch (role)
            {
                case "Student":
                    if (!await _context.Students.AnyAsync(s => s.UserId == user.Id))
                    {
                        _context.Students.Add(new Student
                        {
                            UserId = user.Id,
                            FirstName = profileFirstName,
                            LastName = profileLastName,
                            Email = profileEmail,
                            DateOfBirth = DateTime.UtcNow,
                            Address = "N/A",
                            PhoneNumber = "N/A"
                        });
                        await _context.SaveChangesAsync();
                    }
                    break;
                case "Teacher":
                    if (!await _context.Teachers.AnyAsync(t => t.UserId == user.Id))
                    {
                        _context.Teachers.Add(new Teacher
                        {
                            UserId = user.Id,
                            FirstName = profileFirstName,
                            LastName = profileLastName,
                            Email = profileEmail,
                            Department = "General",
                            PhoneNumber = "N/A"
                        });
                        await _context.SaveChangesAsync();
                    }
                    break;
                case "Parent":
                    if (!await _context.Parents.AnyAsync(p => p.UserId == user.Id))
                    {
                        _context.Parents.Add(new Parent
                        {
                            UserId = user.Id,
                            FirstName = profileFirstName,
                            LastName = profileLastName,
                            Email = profileEmail,
                            Address = "N/A",
                            PhoneNumber = "N/A"
                        });
                        await _context.SaveChangesAsync();
                    }
                    break;
                // No profile needed for Admin role
            }
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

            // Ensure basic info for the profile from the user
            string profileFirstName = user.FirstName ?? string.Empty;
            string profileLastName = user.LastName ?? string.Empty;
            string profileEmail = user.Email ?? string.Empty;

            var student = new Student
            {
                UserId = userId,
                FirstName = profileFirstName,
                LastName = profileLastName,
                Email = profileEmail,
                DateOfBirth = DateTime.UtcNow,
                Address = "N/A",
                PhoneNumber = "N/A"
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
