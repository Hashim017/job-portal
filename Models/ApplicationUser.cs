using Microsoft.AspNetCore.Identity;

namespace JobPortal.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}