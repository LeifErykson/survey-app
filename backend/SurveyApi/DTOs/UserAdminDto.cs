namespace SurveyApi.DTOs;

public class UserAdminDto
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsAdmin { get; set; }
    public int SurveyCount { get; set; }
    public int ResponseCount { get; set; }
}