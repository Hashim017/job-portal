using System.ComponentModel.DataAnnotations;
using JobPortal.Models;

namespace JobPortal.ViewModels;

public class JobFormViewModel : IValidatableObject
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Location { get; set; } = string.Empty;

    [Display(Name = "Job type")]
    public JobType JobType { get; set; }

    [Display(Name = "Minimum salary"), Range(0, 100000000)]
    public decimal? SalaryMin { get; set; }

    [Display(Name = "Maximum salary"), Range(0, 100000000)]
    public decimal? SalaryMax { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (SalaryMin.HasValue && SalaryMax.HasValue && SalaryMax < SalaryMin)
        {
            yield return new ValidationResult(
                "Maximum salary must not be less than minimum salary.",
                new[] { nameof(SalaryMax) });
        }
    }
}