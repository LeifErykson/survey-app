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
public class AnswersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public AnswersController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // POST: api/answers - Create answer option
    [HttpPost]
    public async Task<ActionResult<AnswerOptionDto>> CreateAnswer(CreateAnswerDto createDto)
    {
        // Verify question exists and user owns the survey
        var question = await _context.Questions.FindAsync(createDto.QuestionId);
        if (question == null)
            return NotFound("Question not found");
        
        var survey = await _context.Surveys.FindAsync(question.SurveyId);
        if (survey == null)
            return NotFound("Survey not found");
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (survey.UserId != userId && !User.IsInRole("Admin"))
            return Forbid("You can only add answers to your own surveys");
        
        var answer = new Answer
        {
            Text = createDto.Text,
            QuestionId = createDto.QuestionId
        };
        
        _context.Answers.Add(answer);
        await _context.SaveChangesAsync();
        
        return Ok(new AnswerOptionDto
        {
            Id = answer.Id,
            Text = answer.Text ?? string.Empty
        });
    }
    
    // PUT: api/answers/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAnswer(int id, UpdateAnswerDto updateDto)
    {
        var answer = await _context.Answers.FindAsync(id);
        if (answer == null)
            return NotFound();
        
        // Verify ownership through question and survey
        var question = await _context.Questions.FindAsync(answer.QuestionId);
        if (question == null)
            return NotFound("Question not found");
        var survey = await _context.Surveys.FindAsync(question.SurveyId);
        if (survey == null)
            return NotFound("Survey not found");
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (survey.UserId != userId && !User.IsInRole("Admin"))
            return Forbid("You can only edit answers from your own surveys");
        
        answer.Text = updateDto.Text;
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
    // DELETE: api/answers/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAnswer(int id)
    {
        var answer = await _context.Answers.FindAsync(id);
        if (answer == null)
            return NotFound();
        
        // Verify ownership through question and survey
        var question = await _context.Questions.FindAsync(answer.QuestionId);
        if (question == null)
            return NotFound("Question not found");
        var survey = await _context.Surveys.FindAsync(question.SurveyId);
        if (survey == null)
            return NotFound("Survey not found");

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (survey.UserId != userId && !User.IsInRole("Admin"))
            return Forbid("You can only edit answers from your own surveys");
        
        _context.Answers.Remove(answer);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}