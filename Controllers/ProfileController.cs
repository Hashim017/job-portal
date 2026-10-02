using JobPortal.Data;
using JobPortal.Models;
using JobPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers;

[Authorize(Roles = "JobSeeker")]
public class ProfileController : Controller
{
    private const long MaxSize = 2 * 1024 * 1024;

    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        var resume = await _db.Resumes
            .Where(r => r.UserId == user!.Id && r.IsCurrent)
            .Select(r => new ResumeInfo
            {
                Id = r.Id,
                FileName = r.FileName,
                Size = r.Size,
                UploadedAt = r.UploadedAt
            })
            .FirstOrDefaultAsync();

        return View(new ProfileViewModel
        {
            FullName = user!.FullName,
            Email = user.Email ?? "",
            Resume = resume
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(3_000_000)]
    public async Task<IActionResult> Upload(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Choose a file first.";
            return RedirectToAction(nameof(Index));
        }

        if (file.Length > MaxSize)
        {
            TempData["Error"] = "The file is larger than 2 MB.";
            return RedirectToAction(nameof(Index));
        }

        var name = Path.GetFileName(file.FileName);
        var ext = Path.GetExtension(name).ToLowerInvariant();
        var contentType = ext switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => null
        };

        if (contentType == null)
        {
            TempData["Error"] = "Only PDF, DOC or DOCX files are allowed.";
            return RedirectToAction(nameof(Index));
        }

        byte[] data;
        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms);
            data = ms.ToArray();
        }

        if (!LooksValid(ext, data))
        {
            TempData["Error"] = "This file does not look like a real " + ext.TrimStart('.').ToUpper() + " file.";
            return RedirectToAction(nameof(Index));
        }

        if (name.Length > 200)
        {
            name = name[^200..];
        }

        var userId = _userManager.GetUserId(User)!;

        await _db.Resumes
            .Where(r => r.UserId == userId && r.IsCurrent)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.IsCurrent, false));

        _db.Resumes.Add(new Resume
        {
            UserId = userId,
            FileName = name,
            ContentType = contentType,
            Size = data.Length,
            Data = data,
            IsCurrent = true
        });
        await _db.SaveChangesAsync();

        await CleanupAsync(userId);

        TempData["Success"] = "Resume uploaded.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteResume()
    {
        var userId = _userManager.GetUserId(User)!;

        await _db.Resumes
            .Where(r => r.UserId == userId && r.IsCurrent)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.IsCurrent, false));

        await CleanupAsync(userId);

        TempData["Success"] = "Resume removed.";
        return RedirectToAction(nameof(Index));
    }

    // Old resumes stay only while an application still points to them
    private async Task CleanupAsync(string userId)
    {
        await _db.Resumes
            .Where(r => r.UserId == userId
                        && !r.IsCurrent
                        && !_db.JobApplications.Any(a => a.ResumeId == r.Id))
            .ExecuteDeleteAsync();
    }

    private static bool LooksValid(string ext, byte[] d)
    {
        if (d.Length < 8) return false;

        return ext switch
        {
            ".pdf" => d[0] == 0x25 && d[1] == 0x50 && d[2] == 0x44 && d[3] == 0x46,
            ".docx" => d[0] == 0x50 && d[1] == 0x4B,
            ".doc" => d[0] == 0xD0 && d[1] == 0xCF && d[2] == 0x11 && d[3] == 0xE0,
            _ => false
        };
    }
}