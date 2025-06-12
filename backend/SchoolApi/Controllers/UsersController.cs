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
                    return BadRequest(ModelState);
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
                    return BadRequest(new { Message = "User creation failed.", Errors = result.Errors });
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
        // Delete a user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
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
