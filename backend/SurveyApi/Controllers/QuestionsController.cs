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
public class QuestionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public QuestionsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // POST: api/questions - Create question
    [HttpPost]
    public async Task<ActionResult<QuestionDto>> CreateQuestion(CreateQuestionDto createDto)
    {
        // Verify survey exists and user owns it
        var survey = await _context.Surveys.FindAsync(createDto.SurveyId);
        if (survey == null)
            return NotFound("Survey not found");
        
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (survey.UserId != userId && !User.IsInRole("Admin"))
            return Forbid("You can only add questions to your own surveys");
        
        var question = new Question
        {
            Text = createDto.Text,
            SurveyId = createDto.SurveyId
        };
        
        _context.Questions.Add(question);
        await _context.SaveChangesAsync();
        
        return Ok(new QuestionDto
        {
            Id = question.Id,
            Text = question.Text ?? string.Empty,
            SurveyId = question.SurveyId
        });
    }
    
    // PUT: api/questions/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuestion(int id, UpdateQuestionDto updateDto)
    {
        var question = await _context.Questions.FindAsync(id);
        if (question == null)
            return NotFound();
        
        // Verify ownership through survey
        var survey = await _context.Surveys.FindAsync(question.SurveyId);
        if (survey == null)
            return NotFound("Survey not found");
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (survey.UserId != userId && !User.IsInRole("Admin"))
            return Forbid("You can only edit questions from your own surveys");
        
        question.Text = updateDto.Text;
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
    // DELETE: api/questions/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        var question = await _context.Questions.FindAsync(id);
        if (question == null)
            return NotFound();
        
        // Verify ownership through survey
        var survey = await _context.Surveys.FindAsync(question.SurveyId);
        if (survey == null)
            return NotFound("Survey not found");

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (survey.UserId != userId && !User.IsInRole("Admin"))
            return Forbid("You can only edit questions from your own surveys");
        _context.Questions.Remove(question);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}