namespace JobPortal.Helpers;

public static class UiHelper
{
    public static string Initial(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "?";
        }
        return text.Trim()[0].ToString().ToUpper();
    }

    public static string Salary(decimal? min, decimal? max)
    {
        if (min == null && max == null) return "Salary not shown";
        if (min != null && max != null) return $"PKR {min:N0} - {max:N0}";
        if (min != null) return $"From PKR {min:N0}";
        return $"Up to PKR {max:N0}";
    }

    public static string TimeAgo(DateTime utc)
    {
        var span = DateTime.UtcNow - utc;
        if (span.TotalMinutes < 1) return "Just now";
        if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
        if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
        if (span.TotalDays < 30) return $"{(int)span.TotalDays}d ago";
        return utc.ToString("dd MMM yyyy");
    }

    public static string FileSize(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024.0:0.#} KB";
        return $"{bytes / 1024.0 / 1024.0:0.#} MB";
    }
}