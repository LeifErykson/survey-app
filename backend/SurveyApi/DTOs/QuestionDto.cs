namespace SurveyApi.DTOs;

public class CreateQuestionDto
{
    public string Text { get; set; } = string.Empty;
    public int SurveyId { get; set; }
}

public class UpdateQuestionDto
{
    public string Text { get; set; } = string.Empty;
}

public class QuestionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int SurveyId { get; set; }
}
