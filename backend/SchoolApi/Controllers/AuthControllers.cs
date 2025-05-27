using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SchoolApi.Models;
using SchoolApi.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SchoolApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByNameAsync(dto.Username);
            if (existingUser != null)
            {
                return BadRequest(new { message = "User already exists!" });
            }

            var user = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Email
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                return BadRequest(createResult.Errors);
            }

            if (!await _roleManager.RoleExistsAsync(dto.Role))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(dto.Role));
                if (!roleResult.Succeeded)
                {
                    return StatusCode(500, new { message = "Failed to create role." });
                }
            }

            await _userManager.AddToRoleAsync(user, dto.Role);

            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var secretKey = _configuration["Jwt:SecretKey"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                return StatusCode(500, new { message = "JWT configuration is missing or incomplete." });
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.UtcNow.AddHours(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                username = user.UserName,
                roles = userRoles
            });
        }
    }
}
