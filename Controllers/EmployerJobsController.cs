using JobPortal.Data;
using JobPortal.Models;
using JobPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers;

[Authorize(Roles = "Employer")]
public class EmployerJobsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public EmployerJobsController(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var jobs = await _db.Jobs
            .Where(j => j.EmployerId == userId)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
        return View(jobs);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new JobFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(JobFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var job = new Job
        {
            Title = model.Title.Trim(),
            Description = model.Description.Trim(),
            Location = model.Location.Trim(),
            JobType = model.JobType,
            SalaryMin = model.SalaryMin,
            SalaryMax = model.SalaryMax,
            EmployerId = _userManager.GetUserId(User)!
        };

        _db.Jobs.Add(job);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Job posted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var job = await FindMyJob(id);
        if (job == null)
        {
            return NotFound();
        }

        return View(new JobFormViewModel
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            Location = job.Location,
            JobType = job.JobType,
            SalaryMin = job.SalaryMin,
            SalaryMax = job.SalaryMax
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, JobFormViewModel model)
    {
        var job = await FindMyJob(id);
        if (job == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            model.Id = id;
            return View(model);
        }

        job.Title = model.Title.Trim();
        job.Description = model.Description.Trim();
        job.Location = model.Location.Trim();
        job.JobType = model.JobType;
        job.SalaryMin = model.SalaryMin;
        job.SalaryMax = model.SalaryMax;
        await _db.SaveChangesAsync();

        TempData["Success"] = "Job updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id)
    {
        var job = await FindMyJob(id);
        if (job == null)
        {
            return NotFound();
        }

        job.IsOpen = !job.IsOpen;
        await _db.SaveChangesAsync();

        TempData["Success"] = job.IsOpen ? "Job reopened." : "Job closed.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var job = await FindMyJob(id);
        if (job == null)
        {
            return NotFound();
        }

        _db.Jobs.Remove(job);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Job deleted.";
        return RedirectToAction(nameof(Index));
    }

    private Task<Job?> FindMyJob(int id)
    {
        var userId = _userManager.GetUserId(User);
        return _db.Jobs.FirstOrDefaultAsync(j => j.Id == id && j.EmployerId == userId);
    }
}