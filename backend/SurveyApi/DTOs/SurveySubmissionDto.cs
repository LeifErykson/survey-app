namespace SurveyApi.DTOs;

public class SurveySubmissionDto
{
    public int SurveyId { get; set; }
    public List<QuestionAnswerDto> Answers { get; set; } = new List<QuestionAnswerDto>();
}

public class QuestionAnswerDto
{
    public int QuestionId { get; set; }
    public int AnswerId { get; set; } // Selected answer option
}