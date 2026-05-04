using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyApi.Data;
using SurveyApi.Models;
using SurveyApi.DTOs;

namespace SurveyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SurveysController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public SurveysController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SurveyResponseDto>>> GetSurveys()
    {
        var surveys = await _context.Surveys
            .Select(s => new SurveyResponseDto
            {
                Title = s.Title,
                Description = s.Description,
                CreatedAt = s.CreatedAt,
                IsActive = s.IsActive,
                UserId = s.UserId
            })
            .ToListAsync();
            
        return surveys;
    }
    
    [HttpGet("{id}")]
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
            UserId = survey.UserId
        };
    }
    
    [HttpPost]
    public async Task<ActionResult<SurveyResponseDto>> CreateSurvey(CreateSurveyDto createDto)
    {
        var survey = new Survey
        {
            Title = createDto.Title,
            Description = createDto.Description,
            UserId = createDto.UserId,
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
            UserId = survey.UserId
        });
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSurvey(int id, UpdateSurveyDto updateDto)
    {
        var survey = await _context.Surveys.FindAsync(id);
        if (survey == null)
            return NotFound();
            
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
        
        _context.Surveys.Remove(survey);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}