using JobPortal.Data;
using JobPortal.Models;
using JobPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;

    public DashboardController(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    public IActionResult Index()
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "Admin");
        }

        if (User.IsInRole("Employer"))
        {
            return RedirectToAction(nameof(Employer));
        }

        if (User.IsInRole("JobSeeker"))
        {
            return RedirectToAction(nameof(JobSeeker));
        }

        return RedirectToAction("Index", "Home");
    }

    [Authorize(Roles = "JobSeeker")]
    public async Task<IActionResult> JobSeeker()
    {
        var user = await _userManager.GetUserAsync(User);
        var userId = user!.Id;
        var mine = _db.JobApplications.Where(a => a.ApplicantId == userId);

        var counts = await mine
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        int CountOf(ApplicationStatus status) =>
            counts.FirstOrDefault(c => c.Status == status)?.Count ?? 0;

        var model = new JobSeekerDashboardViewModel
        {
            Profile = user,
            Pending = CountOf(ApplicationStatus.Pending),
            Shortlisted = CountOf(ApplicationStatus.Shortlisted),
            Rejected = CountOf(ApplicationStatus.Rejected),
            Recent = await mine
                .Include(a => a.Job)
                    .ThenInclude(j => j.Employer)
                .OrderByDescending(a => a.AppliedAt)
                .Take(5)
                .ToListAsync()
        };
        model.Total = model.Pending + model.Shortlisted + model.Rejected;

        return View(model);
    }

    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> Employer()
    {
        var user = await _userManager.GetUserAsync(User);
        var userId = user!.Id;

        var myJobs = _db.Jobs.Where(j => j.EmployerId == userId);
        var myApplications = _db.JobApplications.Where(a => a.Job.EmployerId == userId);

        var model = new EmployerDashboardViewModel
        {
            Profile = user,
            TotalJobs = await myJobs.CountAsync(),
            ActiveJobs = await myJobs.CountAsync(j => j.IsOpen),
            TotalApplicants = await myApplications.CountAsync(),
            Shortlisted = await myApplications.CountAsync(a => a.Status == ApplicationStatus.Shortlisted),
            RecentApplicants = await myApplications
                .Include(a => a.Applicant)
                .Include(a => a.Job)
                .OrderByDescending(a => a.AppliedAt)
                .Take(5)
                .ToListAsync()
        };

        return View(model);
    }
}