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
[Authorize]  // Require authentication for all endpoints
public class SurveysController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public SurveysController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    [AllowAnonymous]  // Anyone can view surveys
    public async Task<ActionResult<IEnumerable<SurveyResponseDto>>> GetSurveys()
    {
        var surveys = await _context.Surveys
            .Select(s => new SurveyResponseDto
            {
                Id = s.Id,
                Title = s.Title,
                Description = s.Description,
                CreatedAt = s.CreatedAt,
                IsActive = s.IsActive,
            })
            .ToListAsync();
            
        return surveys;
    }
    
    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SurveyResponseDto>>> GetPublicSurveys()
    {
        var surveys = await _context.Surveys
            .Where(s => s.IsActive)
            .Select(s => new SurveyResponseDto
            {
                Id = s.Id,
                Title = s.Title,
                Description = s.Description,
                CreatedAt = s.CreatedAt,
                IsActive = s.IsActive
            })
            .ToListAsync();
            
        return surveys;
    }

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<SurveyResponseDto>>> GetMySurveys()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var surveys = await _context.Surveys
            .Where(s => s.UserId == userId)
            .Select(s => new SurveyResponseDto
            {
                Id = s.Id,
                Title = s.Title,
                Description = s.Description,
                CreatedAt = s.CreatedAt,
                IsActive = s.IsActive
            })
            .ToListAsync();
            
        return surveys;
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<SurveyResponseDto>> GetSurvey(int id)
    {
        var survey = await _context.Surveys.FindAsync(id);
        
        if (survey == null)
            return NotFound();
            
        return new SurveyResponseDto
        {
            Title = survey.Title,
            Description = survey.Description,
            CreatedAt = survey.CreatedAt,
            IsActive = survey.IsActive,
        };
    }
    
    [HttpPost]
    public async Task<ActionResult<SurveyResponseDto>> CreateSurvey(CreateSurveyDto createDto)
    {
        // Get UserId from JWT token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized("User not found in token");
            
        var userId = int.Parse(userIdClaim.Value);
        
        var survey = new Survey
        {
            Title = createDto.Title,
            Description = createDto.Description,
            UserId = userId,  // From authenticated token, not from DTO
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        
        _context.Surveys.Add(survey);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetSurvey), new { id = survey.Id }, new SurveyResponseDto
        {
            Title = survey.Title,
            Description = survey.Description,
            CreatedAt = survey.CreatedAt,
            IsActive = survey.IsActive,
        });
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSurvey(int id, UpdateSurveyDto updateDto)
    {
        var survey = await _context.Surveys.FindAsync(id);
        if (survey == null)
            return NotFound();
        
        // Check if current user owns this survey
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (survey.UserId != userId && !User.IsInRole("Admin"))
            return Forbid("You can only edit your own surveys");
            
        survey.Title = updateDto.Title;
        survey.Description = updateDto.Description;
        survey.IsActive = updateDto.IsActive;
        
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSurvey(int id)
    {
        var survey = await _context.Surveys.FindAsync(id);
        if (survey == null)
            return NotFound();
        
        // Check if current user owns this survey
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (survey.UserId != userId && !User.IsInRole("Admin"))
            return Forbid("You can only delete your own surveys");
        
        _context.Surveys.Remove(survey);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}