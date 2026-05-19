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
        .OrderByDescending(fs => fs.CreatedAt)
        .ToListAsync();
    
    // Get survey titles
    var surveyIds = responses.Select(r => r.SurveyId).Distinct();
    var surveys = await _context.Surveys
        .Where(s => surveyIds.Contains(s.Id))
        .ToDictionaryAsync(s => s.Id, s => s.Title);
    
    // Get answers for each response
    var result = new List<object>();
    
    foreach (var response in responses)
    {
        var chosenAnswers = await _context.ChoosenAnswers
            .Where(ca => ca.FilledSurveyId == response.Id)
            .ToListAsync();
        
        var answerIds = chosenAnswers.Select(ca => ca.AnswerId).ToList();
        var answers = await _context.Answers
            .Where(a => answerIds.Contains(a.Id))
            .ToListAsync();
        
        var questionIds = answers.Select(a => a.QuestionId).Distinct();
        var questions = await _context.Questions
            .Where(q => questionIds.Contains(q.Id))
            .ToDictionaryAsync(q => q.Id, q => q.Text ?? "Unknown");
        
        result.Add(new
        {
            FilledSurveyId = response.Id,
            SurveyId = response.SurveyId,
            SurveyTitle = surveys.GetValueOrDefault(response.SurveyId, "Unknown"),
            SubmittedAt = response.CreatedAt,
            Answers = answers.Select(a => new
            {
                QuestionId = a.QuestionId,
                QuestionText = questions.GetValueOrDefault(a.QuestionId, "Unknown"),
                AnswerText = a.Text ?? "Unknown"
            })
        });
    }
    
    return Ok(result);
    }
}