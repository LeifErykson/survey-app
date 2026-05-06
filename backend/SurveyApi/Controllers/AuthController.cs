using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SurveyApi.Data;
using SurveyApi.Models;
using SurveyApi.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register(CreateUserDto createDto)
    {
        Console.WriteLine($"Registration attempt: {createDto.Email}");

        // Check if user exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == createDto.Email);
        
        if (existingUser != null)
        {
            Console.WriteLine("User already exists");
            return BadRequest("User with this email already exists");
        }

        var user = new User
        {
            Login = createDto.Login,
            Email = createDto.Email,
            Password = createDto.Password,
            CreatedAt = DateTime.UtcNow,
            IsAdmin = false
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        Console.WriteLine($"User created with ID: {user.Id}");

        return Ok(new UserResponseDto
        {
            Id = user.Id,
            Login = user.Login ?? string.Empty,
            Email = user.Email ?? string.Empty,
            CreatedAt = user.CreatedAt,
            IsAdmin = user.IsAdmin
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.Password == loginDto.Password);

        if (user == null)
            return Unauthorized("Invalid email or password");

        var token = GenerateJwtToken(user);
        return Ok(new { token, user = new UserResponseDto
        {
            Id = user.Id,
            Login = user.Login ?? string.Empty,
            Email = user.Email ?? string.Empty,
            CreatedAt = user.CreatedAt,
            IsAdmin = user.IsAdmin
        } });
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "default-key-32-chars-long-for-dev-only!";
        var key = Encoding.ASCII.GetBytes(jwtKey);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.Login ?? string.Empty),
            new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}