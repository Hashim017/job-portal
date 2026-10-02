using JobPortal.Models;
using Microsoft.AspNetCore.Identity;

namespace JobPortal.Data;

public static class DemoDataSeeder
{
    private const string Password = "Demo@12345";

    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        if (await userManager.FindByEmailAsync("employer1@demo.com") != null)
        {
            return;
        }

        foreach (var role in new[] { "Employer", "JobSeeker" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var companies = new[]
        {
            "TechNova Solutions",
            "CureSoft Health",
            "Pixel Studio",
            "BrightPath Academy",
            "Orbit Logistics"
        };

        var employers = new List<ApplicationUser>();
        for (var i = 0; i < companies.Length; i++)
        {
            var email = $"employer{i + 1}@demo.com";
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = $"HR Manager {i + 1}",
                CompanyName = companies[i],
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Employer");
                employers.Add(user);
            }
        }

        var seekerNames = new[] { "Ali Raza", "Sara Khan", "Usman Tariq", "Ayesha Malik" };
        var seekers = new List<ApplicationUser>();
        for (var i = 0; i < seekerNames.Length; i++)
        {
            var email = $"seeker{i + 1}@demo.com";
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = seekerNames[i],
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "JobSeeker");
                seekers.Add(user);
            }
        }

        if (employers.Count == 0 || seekers.Count == 0)
        {
            return;
        }

        var templates = new (string Title, string Location, int Min, int Max)[]
        {
            ("Junior .NET Developer", "Lahore", 80000, 120000),
            ("Senior ASP.NET Core Developer", "Lahore", 250000, 380000),
            ("Full Stack Developer", "Islamabad", 150000, 230000),
            ("React Frontend Developer", "Remote", 120000, 200000),
            ("UI/UX Designer", "Karachi", 90000, 150000),
            ("Graphic Designer", "Lahore", 60000, 100000),
            ("QA Engineer", "Rawalpindi", 90000, 140000),
            ("DevOps Engineer", "Islamabad", 220000, 340000),
            ("Data Analyst", "Karachi", 110000, 170000),
            ("Marketing Executive", "Faisalabad", 70000, 110000),
            ("Content Writer", "Remote", 50000, 80000),
            ("Digital Marketing Intern", "Lahore", 25000, 35000),
            ("Software Engineering Intern", "Islamabad", 30000, 45000),
            ("Mobile App Developer", "Lahore", 140000, 220000),
            ("Business Analyst", "Karachi", 130000, 200000),
            ("Customer Support Agent", "Gujranwala", 45000, 65000),
            ("Sales Manager", "Karachi", 150000, 250000),
            ("HR Coordinator", "Lahore", 70000, 100000),
            ("Accountant", "Rawalpindi", 80000, 120000),
            ("Project Manager", "Islamabad", 240000, 360000),
            ("Backend Developer", "Remote", 160000, 260000),
            ("Healthcare Software Analyst", "Lahore", 120000, 190000),
            ("Logistics Coordinator", "Faisalabad", 65000, 95000),
            ("Teaching Assistant", "Gujranwala", 40000, 60000)
        };

        var types = Enum.GetValues<JobType>();
        var jobs = new List<Job>();

        for (var i = 0; i < templates.Length; i++)
        {
            var t = templates[i];
            var employer = employers[i % employers.Count];
            var noSalary = i % 7 == 3;

            jobs.Add(new Job
            {
                Title = t.Title,
                Description = Describe(t.Title, employer.CompanyName ?? "our company"),
                Location = t.Location,
                JobType = types[i % types.Length],
                SalaryMin = noSalary ? null : t.Min,
                SalaryMax = noSalary ? null : t.Max,
                IsOpen = i % 6 != 5,
                CreatedAt = DateTime.UtcNow.AddDays(-(i % 20)).AddHours(-i),
                EmployerId = employer.Id
            });
        }

        db.Jobs.AddRange(jobs);
        await db.SaveChangesAsync();

        var notes = new[]
        {
            "I have strong skills for this role and I am ready to start soon.",
            "I love this field and I learn fast. Please consider my application.",
            "I have built several projects that match this job. I would be glad to share them.",
            ""
        };
        var statuses = Enum.GetValues<ApplicationStatus>();

        for (var j = 0; j < jobs.Count; j++)
        {
            var job = jobs[j];
            if (!job.IsOpen)
            {
                continue;
            }

            for (var s = 0; s < seekers.Count; s++)
            {
                if ((j + s) % 3 == 0)
                {
                    continue;
                }

                var applied = job.CreatedAt.AddHours(3 + s * 4);
                if (applied > DateTime.UtcNow)
                {
                    applied = DateTime.UtcNow;
                }

                db.JobApplications.Add(new JobApplication
                {
                    JobId = job.Id,
                    ApplicantId = seekers[s].Id,
                    CoverNote = notes[(j + s) % notes.Length],
                    Status = statuses[(j + s) % statuses.Length],
                    AppliedAt = applied
                });
            }
        }

        await db.SaveChangesAsync();
    }

    private static string Describe(string title, string company)
    {
        return $"{company} is looking for a {title} to join our growing team.\n\n" +
               "What you will do\n" +
               "- Work with a friendly team on real projects\n" +
               "- Share ideas and help improve how we work\n" +
               "- Deliver good quality work on time\n\n" +
               "What we look for\n" +
               "- Good communication skills\n" +
               "- Willingness to learn new things\n" +
               "- Relevant skills or experience for this role";
    }
}