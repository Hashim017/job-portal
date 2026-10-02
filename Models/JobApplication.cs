using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models;

public enum ApplicationStatus
{
    Pending,
    Shortlisted,
    Rejected
}

public static class ApplicationStatusExtensions
{
    public static string CssClass(this ApplicationStatus status) => status switch
    {
        ApplicationStatus.Shortlisted => "status-shortlisted",
        ApplicationStatus.Rejected => "status-rejected",
        _ => "status-pending"
    };
}

public class JobApplication
{
    public int Id { get; set; }

    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public string ApplicantId { get; set; } = string.Empty;
    public ApplicationUser Applicant { get; set; } = null!;

    [MaxLength(2000)]
    public string? CoverNote { get; set; }

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
}