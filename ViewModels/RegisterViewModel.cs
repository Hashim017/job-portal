using System.ComponentModel.DataAnnotations;

namespace JobPortal.ViewModels;

public class RegisterViewModel
{
    [Required, Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8), DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password), Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "JobSeeker";

    [Display(Name = "Company name")]
    public string? CompanyName { get; set; }
}