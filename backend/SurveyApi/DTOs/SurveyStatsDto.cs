namespace SurveyApi.DTOs;

public class SurveyStatsDto
{
    public int SurveyId { get; set; }
    public string SurveyTitle { get; set; } = string.Empty;
    public int TotalResponses { get; set; }
    public List<QuestionStatsDto> Questions { get; set; } = new List<QuestionStatsDto>();
}

public class QuestionStatsDto
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public List<AnswerStatsDto> AnswerStats { get; set; } = new List<AnswerStatsDto>();
}

public class AnswerStatsDto
{
    public int AnswerId { get; set; }
    public string AnswerText { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}