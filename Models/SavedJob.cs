namespace JobPortal.Models;

public class SavedJob
{
    public int Id { get; set; }

    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
}