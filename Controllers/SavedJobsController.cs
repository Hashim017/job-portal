using JobPortal.Data;
using JobPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers;

[Authorize(Roles = "JobSeeker")]
public class SavedJobsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public SavedJobsController(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        var saved = await _db.SavedJobs
            .Include(s => s.Job)
                .ThenInclude(j => j.Employer)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.SavedAt)
            .ToListAsync();

        var jobs = saved.Select(s => s.Job).ToList();
        ViewData["SavedIds"] = jobs.Select(j => j.Id).ToHashSet();

        return View(jobs);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id)
    {
        var userId = _userManager.GetUserId(User)!;

        var existing = await _db.SavedJobs
            .FirstOrDefaultAsync(s => s.JobId == id && s.UserId == userId);

        bool saved;
        if (existing != null)
        {
            _db.SavedJobs.Remove(existing);
            saved = false;
        }
        else
        {
            if (!await _db.Jobs.AnyAsync(j => j.Id == id))
            {
                return NotFound();
            }

            _db.SavedJobs.Add(new SavedJob { JobId = id, UserId = userId });
            saved = true;
        }

        await _db.SaveChangesAsync();

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(new { saved });
        }

        TempData["Success"] = saved ? "Job saved." : "Removed from saved jobs.";

        var referer = Request.Headers.Referer.ToString();
        if (Uri.TryCreate(referer, UriKind.Absolute, out var uri) && Url.IsLocalUrl(uri.PathAndQuery))
        {
            return Redirect(uri.PathAndQuery);
        }

        return RedirectToAction("Details", "Jobs", new { id });
    }
}