namespace SurveyApi.DTOs;

public class CreateAnswerDto
{
    public string Text { get; set; } = string.Empty;
    public int QuestionId { get; set; }
}

public class UpdateAnswerDto
{
    public string Text { get; set; } = string.Empty;
}
