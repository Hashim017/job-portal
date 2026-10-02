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

    public async Task<IActionResult> Index(string? search)
    {
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

        ViewData["Search"] = search;

        var jobs = await query
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();

        return View(jobs);
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

        return View(new JobDetailsViewModel { Job = job, MyApplication = mine });
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