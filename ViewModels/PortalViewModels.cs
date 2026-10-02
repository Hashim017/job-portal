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