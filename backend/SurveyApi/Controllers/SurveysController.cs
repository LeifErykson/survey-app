using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyApi.Data;
using SurveyApi.Models;

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
    public async Task<ActionResult<IEnumerable<Survey>>> GetSurveys()
    {
        return await _context.Surveys.ToListAsync();
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<Survey>> GetSurvey(int id)
    {
        var survey = await _context.Surveys.FindAsync(id);
        
        if (survey == null)
            return NotFound();
            
        return survey;
    }
    [HttpPost]
    public async Task<ActionResult<Survey>> CreateSurvey(Survey survey)
    {
        survey.CreatedAt = DateTime.UtcNow;
        _context.Surveys.Add(survey);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSurvey), new { id = survey.Id }, survey);
   }
}