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
                Login = u.Login ?? string.Empty,
                CreatedAt = u.CreatedAt,
                IsAdmin = u.IsAdmin
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
            Login = user.Login ?? string.Empty,
            CreatedAt = user.CreatedAt,
            IsAdmin = user.IsAdmin
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
            CreatedAt = DateTime.UtcNow,
            IsAdmin = createDto.IsAdmin
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserResponseDto
        {
            Login = user.Login ?? string.Empty,
            CreatedAt = user.CreatedAt,
            IsAdmin = user.IsAdmin
        });
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();
        
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserDto updateDto)
    {
    var user = await _context.Users.FindAsync(id);
    if (user == null)
        return NotFound();
    
    user.Login = updateDto.Login;
    user.Email = updateDto.Email;
    user.Password = updateDto.Password;
    user.IsAdmin = updateDto.IsAdmin;
    
    await _context.SaveChangesAsync();
    
    return NoContent();
}
}