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