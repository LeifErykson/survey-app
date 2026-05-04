using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyApi.Data;
using SurveyApi.Models;
using SurveyApi.DTOs;

namespace SurveyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
    {
        var users = await _context.Users
            .Select(u => new UserResponseDto
            {
                Id = u.Id,
                Login = u.Login ?? string.Empty,
                Email = u.Email ?? string.Empty,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
            
        return users;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDto>> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        
        if (user == null)
            return NotFound();
            
        return new UserResponseDto
        {
            Id = user.Id,
            Login = user.Login ?? string.Empty,
            Email = user.Email ?? string.Empty,
            CreatedAt = user.CreatedAt
        };
    }
    
    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> CreateUser(CreateUserDto createDto)
    {
        var user = new User
        {
            Login = createDto.Login,
            Email = createDto.Email,
            Password = createDto.Password,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserResponseDto
        {
            Id = user.Id,
            Login = user.Login ?? string.Empty,
            Email = user.Email ?? string.Empty,
            CreatedAt = user.CreatedAt
        });
    }
}