namespace SurveyApi.Models;

public class FilledSurvey
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SurveyId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}