using JobPortal.Data;
using JobPortal.Models;
using JobPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers;

public class JobsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public JobsController(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    private const int PageSize = 8;

    public async Task<IActionResult> Index(
        string? search,
        string? location,
        JobType? type,
        decimal? minSalary,
        string? sort,
        int page = 1)
    {
        sort ??= "newest";

        var query = _db.Jobs
            .Include(j => j.Employer)
            .Where(j => j.IsOpen);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(j =>
                j.Title.ToLower().Contains(term) ||
                j.Location.ToLower().Contains(term) ||
                (j.Employer.CompanyName ?? "").ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(j => j.Location == location);
        }

        if (type != null)
        {
            query = query.Where(j => j.JobType == type);
        }

        if (minSalary != null)
        {
            query = query.Where(j => (j.SalaryMax ?? j.SalaryMin) >= minSalary);
        }

        query = sort switch
        {
            "oldest" => query.OrderBy(j => j.CreatedAt),
            "salary_high" => query
                .OrderByDescending(j => j.SalaryMax ?? j.SalaryMin)
                .ThenByDescending(j => j.CreatedAt),
            "title" => query.OrderBy(j => j.Title),
            _ => query.OrderByDescending(j => j.CreatedAt)
        };

        var total = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var jobs = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        var locations = await _db.Jobs
            .Where(j => j.IsOpen)
            .Select(j => j.Location)
            .Distinct()
            .OrderBy(l => l)
            .ToListAsync();

        var model = new JobListViewModel
        {
            Jobs = jobs,
            Search = search,
            Location = location,
            Type = type,
            MinSalary = minSalary,
            Sort = sort,
            Page = page,
            PageSize = PageSize,
            TotalCount = total,
            Locations = locations
        };

        if (User.IsInRole("JobSeeker"))
        {
            var userId = _userManager.GetUserId(User);
            ViewData["SavedIds"] = (await _db.SavedJobs
                .Where(s => s.UserId == userId)
                .Select(s => s.JobId)
                .ToListAsync()).ToHashSet();
        }

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var job = await _db.Jobs
            .Include(j => j.Employer)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
        {
            return NotFound();
        }

        JobApplication? mine = null;
        if (User.IsInRole("JobSeeker"))
        {
            var userId = _userManager.GetUserId(User);
            mine = await _db.JobApplications
                .FirstOrDefaultAsync(a => a.JobId == id && a.ApplicantId == userId);
        }

        var isSaved = false;
        if (User.IsInRole("JobSeeker"))
        {
            var uid = _userManager.GetUserId(User);
            isSaved = await _db.SavedJobs.AnyAsync(s => s.JobId == id && s.UserId == uid);
        }

        return View(new JobDetailsViewModel { Job = job, MyApplication = mine, IsSaved = isSaved });
    }

    [Authorize(Roles = "JobSeeker")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(int id, string? coverNote)
    {
        var job = await _db.Jobs.FirstOrDefaultAsync(j => j.Id == id);
        if (job == null)
        {
            return NotFound();
        }

        if (!job.IsOpen)
        {
            TempData["Error"] = "This job is closed.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var userId = _userManager.GetUserId(User)!;
        var already = await _db.JobApplications
            .AnyAsync(a => a.JobId == id && a.ApplicantId == userId);

        if (already)
        {
            TempData["Error"] = "You already applied to this job.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var note = coverNote?.Trim();
        if (note != null && note.Length > 2000)
        {
            note = note[..2000];
        }

        _db.JobApplications.Add(new JobApplication
        {
            JobId = id,
            ApplicantId = userId,
            CoverNote = note
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Application sent. Good luck!";
        return RedirectToAction(nameof(Details), new { id });
    }
}