using JobPortal.Data;
using JobPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers;

[Authorize(Roles = "JobSeeker")]
public class ApplicationsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ApplicationsController(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var applications = await _db.JobApplications
            .Include(a => a.Job)
                .ThenInclude(j => j.Employer)
            .Where(a => a.ApplicantId == userId)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync();

        return View(applications);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Withdraw(int id)
    {
        var userId = _userManager.GetUserId(User);
        var application = await _db.JobApplications
            .FirstOrDefaultAsync(a => a.Id == id && a.ApplicantId == userId);

        if (application == null)
        {
            return NotFound();
        }

        if (application.Status != ApplicationStatus.Pending)
        {
            TempData["Error"] = "Only pending applications can be withdrawn.";
            return RedirectToAction(nameof(Index));
        }

        _db.JobApplications.Remove(application);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Application withdrawn.";
        return RedirectToAction(nameof(Index));
    }
}