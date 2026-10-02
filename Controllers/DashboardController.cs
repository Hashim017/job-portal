using JobPortal.Data;
using JobPortal.Models;
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
        return View(user);
    }

    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> Employer()
    {
        var user = await _userManager.GetUserAsync(User);

        ViewBag.TotalJobs = await _db.Jobs.CountAsync(j => j.EmployerId == user!.Id);
        ViewBag.ActiveJobs = await _db.Jobs.CountAsync(j => j.EmployerId == user!.Id && j.IsOpen);

        return View(user);
    }
}