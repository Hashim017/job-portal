using JobPortal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<SavedJob> SavedJobs => Set<SavedJob>();
    public DbSet<Resume> Resumes => Set<Resume>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<JobApplication>()
            .HasIndex(a => new { a.JobId, a.ApplicantId })
            .IsUnique();

        builder.Entity<Job>()
    .HasIndex(j => new { j.IsOpen, j.CreatedAt });

        builder.Entity<Job>()
            .HasIndex(j => j.Location);

        builder.Entity<Job>()
            .HasIndex(j => j.JobType);

        builder.Entity<JobApplication>()
            .HasIndex(a => a.ApplicantId);

        builder.Entity<JobApplication>()
            .HasIndex(a => a.Status);

        builder.Entity<SavedJob>()
            .HasIndex(s => new { s.UserId, s.JobId })
            .IsUnique();

        builder.Entity<SavedJob>()
            .HasOne(s => s.Job)
            .WithMany()
            .HasForeignKey(s => s.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<SavedJob>()
            .HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Resume>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Resume>()
            .HasIndex(r => new { r.UserId, r.IsCurrent });

        builder.Entity<JobApplication>()
            .HasOne(a => a.Resume)
            .WithMany()
            .HasForeignKey(a => a.ResumeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}