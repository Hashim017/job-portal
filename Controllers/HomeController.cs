using System.Diagnostics;
using JobPortal.Data;
using JobPortal.Models;
using JobPortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;

    public HomeController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var model = new HomeViewModel
        {
            LatestJobs = await _db.Jobs
                .Include(j => j.Employer)
                .Where(j => j.IsOpen)
                .OrderByDescending(j => j.CreatedAt)
                .Take(6)
                .ToListAsync(),
            OpenJobs = await _db.Jobs.CountAsync(j => j.IsOpen),
            Companies = await _db.Jobs
                .Where(j => j.IsOpen)
                .Select(j => j.EmployerId)
                .Distinct()
                .CountAsync(),
            Applications = await _db.JobApplications.CountAsync()
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}