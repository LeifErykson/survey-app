namespace SurveyApi.DTOs;

public class ResponseHistoryDto
{
    public int FilledSurveyId { get; set; }
    public int SurveyId { get; set; }
    public string SurveyTitle { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public List<AnswerDetailDto> Answers { get; set; } = new List<AnswerDetailDto>();
}

public class AnswerDetailDto
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string AnswerText { get; set; } = string.Empty;
}