namespace SurveyApi.Models;

public class ChoosenAnswers
{
    public int Id { get; set; }
    public int FilledSurveyId { get; set; }
    public int AnswerId { get; set; }
}