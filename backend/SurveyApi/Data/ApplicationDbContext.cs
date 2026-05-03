using Microsoft.EntityFrameworkCore;
using SurveyApi.Models;

namespace SurveyApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Survey> Surveys { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<FilledSurvey> FilledSurveys { get; set; }
    public DbSet<ChoosenAnswers> ChoosenAnswers { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Keep empty or remove entirely - relationships handled by foreign keys in queries
    }
}