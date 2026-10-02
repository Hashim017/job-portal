using System.Net;
using JobPortal.Models;

namespace JobPortal.Services;

public static class EmailTemplates
{
    public static (string Subject, string Html, string Text) StatusChanged(
        string name,
        string jobTitle,
        string company,
        ApplicationStatus status,
        string link)
    {
        var shortlisted = status == ApplicationStatus.Shortlisted;

        var subject = shortlisted
            ? $"Good news: you are shortlisted for {jobTitle}"
            : $"Update on your application for {jobTitle}";

        var headline = shortlisted ? "You are shortlisted" : "Application update";

        var message = shortlisted
            ? $"{company} liked your application for {jobTitle} and moved you to the shortlist. They may contact you soon."
            : $"{company} reviewed your application for {jobTitle} and will not move forward this time. Keep going. The right job is out there.";

        var color = shortlisted ? "#15803d" : "#b91c1c";

        var html = $"""
<div style="background:#f5f6fb;padding:24px;font-family:Arial,Helvetica,sans-serif;">
  <div style="max-width:520px;margin:0 auto;background:#ffffff;border-radius:16px;overflow:hidden;border:1px solid #e8e9f3;">
    <div style="background:#5b5bf0;padding:22px 28px;color:#ffffff;font-size:20px;font-weight:bold;">JobPortal</div>
    <div style="padding:28px;color:#1e1b4b;line-height:1.6;">
      <p style="margin:0 0 12px;">Hi {WebUtility.HtmlEncode(name)},</p>
      <h2 style="margin:0 0 12px;color:{color};">{headline}</h2>
      <p style="margin:0 0 20px;">{WebUtility.HtmlEncode(message)}</p>
      <p style="margin:0 0 24px;"><strong>{WebUtility.HtmlEncode(jobTitle)}</strong><br>{WebUtility.HtmlEncode(company)}</p>
      <a href="{WebUtility.HtmlEncode(link)}" style="display:inline-block;background:#5b5bf0;color:#ffffff;text-decoration:none;padding:12px 24px;border-radius:999px;font-weight:bold;">View my applications</a>
    </div>
    <div style="padding:16px 28px;color:#6b7280;font-size:12px;border-top:1px solid #e8e9f3;">You got this email because you applied for a job on JobPortal.</div>
  </div>
</div>
""";

        var text = $"Hi {name},\n\n{headline}\n\n{message}\n\nJob: {jobTitle} at {company}\n\nView your applications: {link}\n";

        return (subject, html, text);
    }
}