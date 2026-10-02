using JobPortal.Data;
using JobPortal.Models;
using JobPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var roleCounts = await (
            from ur in _db.UserRoles
            join r in _db.Roles on ur.RoleId equals r.Id
            group r by r.Name into g
            select new { Role = g.Key, Count = g.Count() })
            .ToListAsync();

        int RoleCount(string role) =>
            roleCounts.FirstOrDefault(r => r.Role == role)?.Count ?? 0;

        var model = new AdminDashboardViewModel
        {
            TotalUsers = await _userManager.Users.CountAsync(),
            JobSeekers = RoleCount("JobSeeker"),
            Employers = RoleCount("Employer"),
            TotalJobs = await _db.Jobs.CountAsync(),
            OpenJobs = await _db.Jobs.CountAsync(j => j.IsOpen),
            TotalApplications = await _db.JobApplications.CountAsync(),
            RecentJobs = await _db.Jobs
                .Include(j => j.Employer)
                .OrderByDescending(j => j.CreatedAt)
                .Take(5)
                .ToListAsync(),
            RecentApplications = await _db.JobApplications
                .Include(a => a.Applicant)
                .Include(a => a.Job)
                .OrderByDescending(a => a.AppliedAt)
                .Take(5)
                .ToListAsync()
        };

        return View(model);
    }

    public async Task<IActionResult> Users(string? search, string? role)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(u =>
                (u.Email ?? "").ToLower().Contains(term) ||
                (u.FullName ?? "").ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            var ids = from ur in _db.UserRoles
                      join r in _db.Roles on ur.RoleId equals r.Id
                      where r.Name == role
                      select ur.UserId;
            query = query.Where(u => ids.Contains(u.Id));
        }

        var users = await query
            .OrderBy(u => u.FullName)
            .Take(200)
            .ToListAsync();

        var userIds = users.Select(u => u.Id).ToList();
        var roleRows = await (
            from ur in _db.UserRoles
            join r in _db.Roles on ur.RoleId equals r.Id
            where userIds.Contains(ur.UserId)
            select new { ur.UserId, r.Name })
            .ToListAsync();

        var rows = users.Select(u => new AdminUserRow
        {
            User = u,
            Role = roleRows.FirstOrDefault(r => r.UserId == u.Id)?.Name ?? "None",
            IsLocked = u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow
        }).ToList();

        return View(new AdminUsersViewModel { Rows = rows, Search = search, Role = role });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLock(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        if (user.Id == _userManager.GetUserId(User) || await _userManager.IsInRoleAsync(user, "Admin"))
        {
            TempData["Error"] = "Admin accounts cannot be locked.";
            return RedirectToAction(nameof(Users));
        }

        var locked = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow;
        if (locked)
        {
            await _userManager.SetLockoutEndDateAsync(user, null);
            TempData["Success"] = "User unlocked.";
        }
        else
        {
            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
            await _userManager.UpdateSecurityStampAsync(user);
            TempData["Success"] = "User locked.";
        }

        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        if (user.Id == _userManager.GetUserId(User) || await _userManager.IsInRoleAsync(user, "Admin"))
        {
            TempData["Error"] = "Admin accounts cannot be deleted.";
            return RedirectToAction(nameof(Users));
        }

        var savedByUser = await _db.SavedJobs.Where(s => s.UserId == id).ToListAsync();
        _db.SavedJobs.RemoveRange(savedByUser);

        var apps = await _db.JobApplications
            .Where(a => a.ApplicantId == id || a.Job.EmployerId == id)
            .ToListAsync();
        _db.JobApplications.RemoveRange(apps);

        var userResumes = await _db.Resumes.Where(r => r.UserId == id).ToListAsync();
        _db.Resumes.RemoveRange(userResumes);

        var jobs = await _db.Jobs.Where(j => j.EmployerId == id).ToListAsync();
        _db.Jobs.RemoveRange(jobs);

        await _db.SaveChangesAsync();
        await _userManager.DeleteAsync(user);

        TempData["Success"] = "User deleted.";
        return RedirectToAction(nameof(Users));
    }

    public async Task<IActionResult> Jobs(string? search, string? status)
    {
        var query = _db.Jobs
            .Include(j => j.Employer)
            .Include(j => j.Applications)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(j =>
                j.Title.ToLower().Contains(term) ||
                j.Location.ToLower().Contains(term) ||
                (j.Employer.CompanyName ?? "").ToLower().Contains(term));
        }

        if (status == "open")
        {
            query = query.Where(j => j.IsOpen);
        }
        else if (status == "closed")
        {
            query = query.Where(j => !j.IsOpen);
        }

        var jobs = await query
            .OrderByDescending(j => j.CreatedAt)
            .Take(200)
            .ToListAsync();

        return View(new AdminJobsViewModel { Jobs = jobs, Search = search, Status = status });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleJob(int id)
    {
        var job = await _db.Jobs.FirstOrDefaultAsync(j => j.Id == id);
        if (job == null)
        {
            return NotFound();
        }

        job.IsOpen = !job.IsOpen;
        await _db.SaveChangesAsync();

        TempData["Success"] = job.IsOpen ? "Job reopened." : "Job closed.";
        return RedirectToAction(nameof(Jobs));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteJob(int id)
    {
        var job = await _db.Jobs
            .Include(j => j.Applications)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
        {
            return NotFound();
        }

        _db.JobApplications.RemoveRange(job.Applications);
        _db.Jobs.Remove(job);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Job deleted.";
        return RedirectToAction(nameof(Jobs));
    }
}