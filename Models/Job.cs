using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models;

public enum JobType
{
    FullTime,
    PartTime,
    Contract,
    Internship
}

public static class JobTypeExtensions
{
    public static string Label(this JobType type) => type switch
    {
        JobType.FullTime => "Full time",
        JobType.PartTime => "Part time",
        JobType.Contract => "Contract",
        JobType.Internship => "Internship",
        _ => type.ToString()
    };
}

public class Job
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Location { get; set; } = string.Empty;

    public JobType JobType { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public bool IsOpen { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string EmployerId { get; set; } = string.Empty;
    public ApplicationUser Employer { get; set; } = null!;
    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
}