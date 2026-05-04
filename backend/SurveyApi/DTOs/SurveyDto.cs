namespace SurveyApi.DTOs;

public class CreateSurveyDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int UserId { get; set; }  // Temporary - will come from JWT later
}

public class UpdateSurveyDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class SurveyResponseDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public int UserId { get; set; }
}