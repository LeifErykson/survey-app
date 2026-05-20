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

    [HttpGet("{id}/details")]
    [AllowAnonymous]
    public async Task<ActionResult<SurveyDetailDto>> GetSurveyDetails(int id)
    {
    var survey = await _context.Surveys
        .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);
    
    if (survey == null)
        return NotFound("Survey not found or inactive");
    
    var questions = await _context.Questions
        .Where(q => q.SurveyId == id)
        .ToListAsync();
    
    var questionIds = questions.Select(q => q.Id).ToList();
    
    var answers = await _context.Answers
        .Where(a => questionIds.Contains(a.QuestionId))
        .ToListAsync();
    
    var result = new SurveyDetailDto
    {
        Id = survey.Id,
        Title = survey.Title,
        Description = survey.Description,
        IsActive = survey.IsActive,
        Questions = questions.Select(q => new QuestionDetailDto
        {
            Id = q.Id,
            Text = q.Text ?? string.Empty,
            Options = answers
                .Where(a => a.QuestionId == q.Id)
                .Select(a => new AnswerOptionDto
                {
                    Id = a.Id,
                    Text = a.Text ?? string.Empty
                }).ToList(),
            Type = q.Type
        }).ToList()
    };
    
    return Ok(result);
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
    
    [HttpGet("{id}/stats")]
    public async Task<ActionResult<SurveyStatsDto>> GetSurveyStats(int id)
    {
    // Verify survey exists and user owns it
    var survey = await _context.Surveys.FindAsync(id);
    if (survey == null)
        return NotFound("Survey not found");
    
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    if (survey.UserId != userId && !User.IsInRole("Admin"))
        return Forbid("You can only view stats for your own surveys");
    
    // Get all responses for this survey
    var filledSurveys = await _context.FilledSurveys
        .Where(fs => fs.SurveyId == id)
        .ToListAsync();
    
    var totalResponses = filledSurveys.Count;
    
    // Get all questions for this survey
    var questions = await _context.Questions
        .Where(q => q.SurveyId == id)
        .ToListAsync();
    
    // Get all chosen answers for responses
    var filledSurveyIds = filledSurveys.Select(fs => fs.Id).ToList();
    var chosenAnswers = await _context.ChoosenAnswers
        .Where(ca => filledSurveyIds.Contains(ca.FilledSurveyId))
        .ToListAsync();
    
    var answerIds = chosenAnswers.Select(ca => ca.AnswerId).Distinct().ToList();
    var answers = await _context.Answers
        .Where(a => answerIds.Contains(a.Id))
        .ToDictionaryAsync(a => a.Id, a => a);
    
    // Build stats per question
    var questionStats = new List<QuestionStatsDto>();
    
    foreach (var question in questions)
    {
        // Get answers for this question
        var questionAnswers = await _context.Answers
            .Where(a => a.QuestionId == question.Id)
            .ToListAsync();
        
        var answerStats = new List<AnswerStatsDto>();
        
        foreach (var answer in questionAnswers)
        {
            var count = chosenAnswers.Count(ca => ca.AnswerId == answer.Id);
            var percentage = totalResponses > 0 ? (count * 100.0 / totalResponses) : 0;
            
            answerStats.Add(new AnswerStatsDto
            {
                AnswerId = answer.Id,
                AnswerText = answer.Text ?? string.Empty,
                Count = count,
                Percentage = Math.Round(percentage, 1)
            });
        }
        
        questionStats.Add(new QuestionStatsDto
        {
            QuestionId = question.Id,
            QuestionText = question.Text ?? string.Empty,
            QuestionType = question.Type ?? "single",
            AnswerStats = answerStats
        });
    }
    
    var result = new SurveyStatsDto
    {
        SurveyId = survey.Id,
        SurveyTitle = survey.Title,
        TotalResponses = totalResponses,
        Questions = questionStats
    };
    
    return Ok(result);
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