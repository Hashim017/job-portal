using JobPortal.Models;

namespace JobPortal.ViewModels;

public class HomeViewModel
{
    public List<Job> LatestJobs { get; set; } = new();
    public int OpenJobs { get; set; }
    public int Companies { get; set; }
    public int Applications { get; set; }
}

public class JobDetailsViewModel
{
    public Job Job { get; set; } = null!;
    public JobApplication? MyApplication { get; set; }
}

public class JobSeekerDashboardViewModel
{
    public ApplicationUser Profile { get; set; } = null!;
    public int Total { get; set; }
    public int Pending { get; set; }
    public int Shortlisted { get; set; }
    public int Rejected { get; set; }
    public List<JobApplication> Recent { get; set; } = new();
}

public class EmployerDashboardViewModel
{
    public ApplicationUser Profile { get; set; } = null!;
    public int TotalJobs { get; set; }
    public int ActiveJobs { get; set; }
    public int TotalApplicants { get; set; }
    public int Shortlisted { get; set; }
    public List<JobApplication> RecentApplicants { get; set; } = new();
}

public class JobListViewModel
{
    public List<Job> Jobs { get; set; } = new();
    public string? Search { get; set; }
    public string? Location { get; set; }
    public JobType? Type { get; set; }
    public decimal? MinSalary { get; set; }
    public string Sort { get; set; } = "newest";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 8;
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public List<string> Locations { get; set; } = new();

    public bool HasFilters =>
        !string.IsNullOrWhiteSpace(Search) ||
        !string.IsNullOrWhiteSpace(Location) ||
        Type != null ||
        MinSalary != null;
}

public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int JobSeekers { get; set; }
    public int Employers { get; set; }
    public int TotalJobs { get; set; }
    public int OpenJobs { get; set; }
    public int TotalApplications { get; set; }
    public List<Job> RecentJobs { get; set; } = new();
    public List<JobApplication> RecentApplications { get; set; } = new();
}

public class AdminUserRow
{
    public ApplicationUser User { get; set; } = null!;
    public string Role { get; set; } = "None";
    public bool IsLocked { get; set; }
}

public class AdminUsersViewModel
{
    public List<AdminUserRow> Rows { get; set; } = new();
    public string? Search { get; set; }
    public string? Role { get; set; }
}

public class AdminJobsViewModel
{
    public List<Job> Jobs { get; set; } = new();
    public string? Search { get; set; }
    public string? Status { get; set; }
}