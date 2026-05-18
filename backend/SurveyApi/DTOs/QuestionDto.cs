namespace SurveyApi.DTOs;

public class CreateQuestionDto
{
    public string Text { get; set; } = string.Empty;
    public int SurveyId { get; set; }
    public string Type { get; set; } = "single";
}

public class UpdateQuestionDto
{
    public string Text { get; set; } = string.Empty;
    public string Type { get; set; } = "single";
}

public class QuestionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int SurveyId { get; set; }
    public string Type { get; set; } = string.Empty;
}
