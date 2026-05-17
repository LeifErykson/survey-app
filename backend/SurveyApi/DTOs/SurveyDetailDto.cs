namespace SurveyApi.DTOs;

public class SurveyDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<QuestionDetailDto> Questions { get; set; } = new List<QuestionDetailDto>();
}

public class QuestionDetailDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public List<AnswerOptionDto> Options { get; set; } = new List<AnswerOptionDto>();
}

public class AnswerOptionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
}