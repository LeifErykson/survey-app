namespace SurveyApi.Models;

public class Question
{
    public int Id { get; set; }
    public string? Text { get; set; }
    public int SurveyId { get; set; }
}