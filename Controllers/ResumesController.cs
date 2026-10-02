using JobPortal.Data;
using JobPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers;

[Authorize]
public class ResumesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ResumesController(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Download(int id)
    {
        var userId = _userManager.GetUserId(User);

        var resume = await _db.Resumes
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (resume == null)
        {
            return NotFound();
        }

        var allowed =
            resume.UserId == userId ||
            User.IsInRole("Admin") ||
            await _db.JobApplications.AnyAsync(a =>
                a.ResumeId == id && a.Job.EmployerId == userId);

        if (!allowed)
        {
            return Forbid();
        }

        return File(resume.Data, resume.ContentType, resume.FileName);
    }
}