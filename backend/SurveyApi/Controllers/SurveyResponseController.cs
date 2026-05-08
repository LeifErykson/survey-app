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
[Authorize]
public class SurveyResponsesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public SurveyResponsesController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpPost("submit")]
    public async Task<IActionResult> SubmitSurvey(SurveySubmissionDto submission)
    {
        // Get current user ID from JWT
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        // Check if survey exists and is active
        var survey = await _context.Surveys
            .FirstOrDefaultAsync(s => s.Id == submission.SurveyId && s.IsActive);
        
        if (survey == null)
            return BadRequest("Survey not found or inactive");
        
        // Create filled survey record
        var filledSurvey = new FilledSurvey
        {
            UserId = userId,
            SurveyId = submission.SurveyId,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.FilledSurveys.Add(filledSurvey);
        await _context.SaveChangesAsync();
        
        // Save each answer
        foreach (var answer in submission.Answers)
        {
            var choosenAnswer = new ChoosenAnswers
            {
                FilledSurveyId = filledSurvey.Id,
                AnswerId = answer.AnswerId
            };
            _context.ChoosenAnswers.Add(choosenAnswer);
        }
        
        await _context.SaveChangesAsync();
        
        return Ok(new { message = "Survey submitted successfully", filledSurveyId = filledSurvey.Id });
    }
    
    [HttpGet("my-responses")]
    public async Task<IActionResult> GetMyResponses()
    {
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    
    var responses = await _context.FilledSurveys
        .Where(fs => fs.UserId == userId)
        .Select(fs => new
        {
            fs.Id,
            fs.SurveyId,
            fs.CreatedAt
        })
        .ToListAsync();
    
    // Get survey titles separately if needed
    var surveyIds = responses.Select(r => r.SurveyId).Distinct();
    var surveys = await _context.Surveys
        .Where(s => surveyIds.Contains(s.Id))
        .ToDictionaryAsync(s => s.Id, s => s.Title);
    
    var result = responses.Select(r => new
    {
        r.Id,
        r.SurveyId,
        SurveyTitle = surveys.ContainsKey(r.SurveyId) ? surveys[r.SurveyId] : "Unknown",
        r.CreatedAt
    });
    
    return Ok(result);
    }
}