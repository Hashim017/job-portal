using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? Headline { get; set; }

    [MaxLength(300)]
    public string? Skills { get; set; }

    [MaxLength(100)]
    public string? Industry { get; set; }

    [MaxLength(200)]
    public string? Website { get; set; }

    [MaxLength(1500)]
    public string? About { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}