using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyApi.Data;
using SurveyApi.Models;
using SurveyApi.DTOs;
using System.Security.Claims;

namespace SurveyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet("users")]
    public async Task<ActionResult<List<UserAdminDto>>> GetAllUsers()
    {
        var users = await _context.Users
            .Select(u => new UserAdminDto
            {
                Id = u.Id,
                Login = u.Login ?? string.Empty,
                Email = u.Email ?? string.Empty,
                CreatedAt = u.CreatedAt,
                IsAdmin = u.IsAdmin,
                SurveyCount = _context.Surveys.Count(s => s.UserId == u.Id),
                ResponseCount = _context.FilledSurveys.Count(fs => fs.UserId == u.Id)
            })
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
        
        return Ok(users);
    }
    
    [HttpPut("users/{id}/make-admin")]
    public async Task<IActionResult> MakeAdmin(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound("User not found");
        
        // Don't let admin remove their own admin status through this endpoint
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (currentUserId == id)
            return BadRequest("You cannot change your own admin status here");
        
        user.IsAdmin = true;
        await _context.SaveChangesAsync();
        
        return Ok(new { message = $"User {user.Login} is now an admin" });
    }
    
    [HttpPut("users/{id}/remove-admin")]
    public async Task<IActionResult> RemoveAdmin(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound("User not found");
        
        // Prevent removing admin from the last admin
        var adminCount = await _context.Users.CountAsync(u => u.IsAdmin);
        if (adminCount <= 1 && user.IsAdmin)
            return BadRequest("Cannot remove the last admin user");
        
        user.IsAdmin = false;
        await _context.SaveChangesAsync();
        
        return Ok(new { message = $"Admin rights removed from {user.Login}" });
    }
    
    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound("User not found");
        
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (currentUserId == id)
            return BadRequest("You cannot delete your own account");
        
        // Delete user's surveys and responses first (foreign key constraints)
        var userSurveys = await _context.Surveys.Where(s => s.UserId == id).ToListAsync();
        foreach (var survey in userSurveys)
        {
            var responses = await _context.FilledSurveys.Where(fs => fs.SurveyId == survey.Id).ToListAsync();
            _context.FilledSurveys.RemoveRange(responses);
        }
        _context.Surveys.RemoveRange(userSurveys);
        
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        
        return Ok(new { message = $"User {user.Login} deleted successfully" });
    }
}