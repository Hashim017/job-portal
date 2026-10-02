using JobPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
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
        return View(user);
    }
}