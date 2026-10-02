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
    }
}