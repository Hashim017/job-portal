using System.Text;
using JobPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Data;

public static class DemoExtrasSeeder
{
    private const string Password = "Demo@12345";

    private static readonly string[] SeekerNames =
    {
        "Ahmed Khan", "Fatima Noor", "Bilal Ahmed", "Hina Siddiqui", "Hamza Sheikh",
        "Zainab Ali", "Hassan Raza", "Maryam Iqbal", "Daniyal Butt", "Sana Javed",
        "Omar Farooq", "Noor Fatima", "Talha Mehmood", "Iqra Hussain", "Saad Qureshi",
        "Laiba Aslam", "Faizan Ahmad", "Rabia Anwar", "Shahzaib Malik", "Mahnoor Tariq"
    };

    private static readonly (string Headline, string Skills)[] SeekerProfiles =
    {
        ("Junior .NET Developer", "C#, ASP.NET Core, SQL Server, Git"),
        ("Frontend Developer", "HTML, CSS, JavaScript, React"),
        ("UI/UX Designer", "Figma, Prototyping, User Research"),
        ("Data Analyst", "SQL, Excel, Power BI, Python"),
        ("QA Engineer", "Manual Testing, Selenium, Postman"),
        ("Digital Marketer", "SEO, Content Writing, Social Media"),
        ("Backend Developer", "Node.js, PostgreSQL, REST APIs, Docker"),
        ("Business Analyst", "Requirements, Documentation, Agile")
    };

    private static readonly string[] Cities =
    {
        "Lahore", "Islamabad", "Karachi", "Rawalpindi", "Faisalabad", "Gujranwala", "Multan"
    };

    private static readonly string[] NewCompanies =
    {
        "Nexus Software House", "MediCare Plus", "GreenLeaf Foods",
        "Skyline Real Estate", "EduSpark Learning", "Swift Courier"
    };

    private static readonly string[] Industries =
    {
        "Software", "Healthcare", "Design", "Education", "Logistics",
        "Software", "Healthcare", "Food and Beverage", "Real Estate", "Education", "Logistics"
    };

    private static readonly string[] EmployerCities =
    {
        "Lahore", "Lahore", "Karachi", "Gujranwala", "Faisalabad",
        "Lahore", "Islamabad", "Karachi", "Lahore", "Rawalpindi", "Faisalabad"
    };

    private static readonly (string Title, string Location, int Min, int Max)[] JobTemplates =
    {
        ("Software Engineer", "Lahore", 130000, 200000),
        ("Associate Software Engineer", "Lahore", 80000, 120000),
        ("Angular Developer", "Islamabad", 140000, 210000),
        ("Python Developer", "Remote", 150000, 240000),
        ("Flutter Developer", "Lahore", 130000, 200000),

        ("Medical Coder", "Islamabad", 60000, 90000),
        ("Healthcare Data Analyst", "Islamabad", 110000, 170000),
        ("Clinic Operations Manager", "Lahore", 130000, 190000),
        ("Pharmacy Software Tester", "Lahore", 80000, 120000),
        ("Nurse Coordinator", "Islamabad", 70000, 100000),

        ("Restaurant Manager", "Karachi", 90000, 140000),
        ("Quality Control Officer", "Karachi", 65000, 95000),
        ("Supply Chain Executive", "Karachi", 85000, 130000),
        ("Store Supervisor", "Lahore", 55000, 80000),
        ("Food Safety Inspector", "Karachi", 70000, 105000),

        ("Property Consultant", "Lahore", 60000, 200000),
        ("Real Estate Sales Manager", "Lahore", 150000, 260000),
        ("Architect Assistant", "Lahore", 70000, 110000),
        ("Interior Designer", "Islamabad", 80000, 130000),
        ("Marketing Coordinator", "Lahore", 70000, 105000),

        ("Math Teacher", "Rawalpindi", 50000, 75000),
        ("Physics Lecturer", "Rawalpindi", 90000, 140000),
        ("Curriculum Developer", "Remote", 100000, 160000),
        ("Student Counselor", "Rawalpindi", 55000, 80000),
        ("Online Tutor", "Remote", 40000, 70000),

        ("Delivery Operations Lead", "Faisalabad", 90000, 140000),
        ("Fleet Coordinator", "Faisalabad", 75000, 110000),
        ("Warehouse Manager", "Faisalabad", 100000, 150000),
        ("Customer Support Agent", "Faisalabad", 45000, 65000),
        ("Route Planner", "Faisalabad", 60000, 90000)
    };

    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        if (await userManager.FindByEmailAsync("employer11@demo.com") != null)
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

        // New employers 6 to 11
        for (var i = 0; i < NewCompanies.Length; i++)
        {
            var n = 6 + i;
            var email = $"employer{n}@demo.com";
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = $"Hiring Team {n}",
                CompanyName = NewCompanies[i],
                EmailConfirmed = true
            };
            ApplyEmployerProfile(user, n);

            var result = await userManager.CreateAsync(user, Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Employer");
            }
        }

        // New job seekers 5 to 24
        for (var i = 0; i < SeekerNames.Length; i++)
        {
            var n = 5 + i;
            var email = $"seeker{n}@demo.com";
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = SeekerNames[i],
                EmailConfirmed = true
            };
            ApplySeekerProfile(user, n);

            var result = await userManager.CreateAsync(user, Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "JobSeeker");
            }
        }

        // Fill profiles of the first batch of demo users
        var demoUsers = await userManager.Users
            .Where(u => u.Email != null && u.Email.EndsWith("@demo.com"))
            .ToListAsync();

        foreach (var u in demoUsers.Where(u => string.IsNullOrWhiteSpace(u.About)))
        {
            var email = u.Email!;
            if (email.StartsWith("seeker"))
            {
                ApplySeekerProfile(u, ParseNumber(email, "seeker"));
                await userManager.UpdateAsync(u);
            }
            else if (email.StartsWith("employer"))
            {
                ApplyEmployerProfile(u, ParseNumber(email, "employer"));
                await userManager.UpdateAsync(u);
            }
        }

        // New jobs
        var newEmployers = new List<ApplicationUser>();
        for (var n = 6; n <= 11; n++)
        {
            var employer = await userManager.FindByEmailAsync($"employer{n}@demo.com");
            if (employer != null)
            {
                newEmployers.Add(employer);
            }
        }

        if (newEmployers.Count == 0)
        {
            return;
        }

        var types = Enum.GetValues<JobType>();
        var jobs = new List<Job>();
        for (var i = 0; i < JobTemplates.Length; i++)
        {
            var t = JobTemplates[i];
            var employer = newEmployers[Math.Min(i / 5, newEmployers.Count - 1)];
            var noSalary = i % 9 == 4;

            jobs.Add(new Job
            {
                Title = t.Title,
                Description = Describe(t.Title, employer.CompanyName ?? "our company"),
                Location = t.Location,
                JobType = types[i % types.Length],
                SalaryMin = noSalary ? null : t.Min,
                SalaryMax = noSalary ? null : t.Max,
                IsOpen = i % 8 != 7,
                CreatedAt = DateTime.UtcNow.AddDays(-(i % 25)).AddHours(-(i * 3 % 20)),
                EmployerId = employer.Id
            });
        }

        db.Jobs.AddRange(jobs);
        await db.SaveChangesAsync();

        // Resumes for every demo job seeker
        var seekers = await userManager.Users
            .Where(u => u.Email != null && u.Email.StartsWith("seeker") && u.Email.EndsWith("@demo.com"))
            .ToListAsync();

        var haveResume = (await db.Resumes.Select(r => r.UserId).Distinct().ToListAsync()).ToHashSet();
        foreach (var seeker in seekers.Where(s => !haveResume.Contains(s.Id)))
        {
            var data = MakeResume(seeker);
            db.Resumes.Add(new Resume
            {
                UserId = seeker.Id,
                FileName = seeker.FullName.Replace(' ', '_') + "_Resume.pdf",
                ContentType = "application/pdf",
                Size = data.Length,
                Data = data,
                IsCurrent = true
            });
        }
        await db.SaveChangesAsync();

        var resumeRows = await db.Resumes
            .Where(r => r.IsCurrent)
            .Select(r => new { r.UserId, r.Id })
            .ToListAsync();
        var resumeMap = resumeRows
            .GroupBy(r => r.UserId)
            .ToDictionary(g => g.Key, g => g.First().Id);

        // Applications and saved jobs
        var rng = new Random(7);
        var openJobs = await db.Jobs.Where(j => j.IsOpen).ToListAsync();

        var existingApps = (await db.JobApplications
                .Select(a => new { a.JobId, a.ApplicantId })
                .ToListAsync())
            .Select(a => (a.JobId, a.ApplicantId))
            .ToHashSet();

        var existingSaved = (await db.SavedJobs
                .Select(s => new { s.JobId, s.UserId })
                .ToListAsync())
            .Select(s => (s.JobId, s.UserId))
            .ToHashSet();

        var notes = new[]
        {
            "I have strong skills for this role and I am ready to start soon.",
            "I love this field and I learn fast. Please consider my application.",
            "I have built several projects that match this job. I would be glad to share them.",
            ""
        };

        foreach (var seeker in seekers)
        {
            var appCount = rng.Next(3, 7);
            foreach (var job in openJobs.OrderBy(_ => rng.Next()).Take(appCount))
            {
                if (!existingApps.Add((job.Id, seeker.Id)))
                {
                    continue;
                }

                var roll = rng.Next(100);
                var status = roll < 55
                    ? ApplicationStatus.Pending
                    : roll < 80 ? ApplicationStatus.Shortlisted : ApplicationStatus.Rejected;

                var applied = job.CreatedAt.AddHours(rng.Next(2, 72));
                if (applied > DateTime.UtcNow)
                {
                    applied = DateTime.UtcNow.AddMinutes(-rng.Next(5, 600));
                }

                db.JobApplications.Add(new JobApplication
                {
                    JobId = job.Id,
                    ApplicantId = seeker.Id,
                    CoverNote = notes[rng.Next(notes.Length)],
                    Status = status,
                    AppliedAt = applied,
                    ResumeId = resumeMap.TryGetValue(seeker.Id, out var rid) ? rid : (int?)null
                });
            }

            var saveCount = rng.Next(2, 6);
            foreach (var job in openJobs.OrderBy(_ => rng.Next()).Take(saveCount))
            {
                if (!existingSaved.Add((job.Id, seeker.Id)))
                {
                    continue;
                }

                db.SavedJobs.Add(new SavedJob
                {
                    JobId = job.Id,
                    UserId = seeker.Id,
                    SavedAt = DateTime.UtcNow.AddHours(-rng.Next(1, 300))
                });
            }
        }

        await db.SaveChangesAsync();
    }

    private static string Phone(int n) => $"+92 300 {1234000 + n * 137}";

    private static int ParseNumber(string email, string prefix)
    {
        var at = email.IndexOf('@');
        return int.TryParse(email[prefix.Length..at], out var n) ? n : 1;
    }

    private static void ApplySeekerProfile(ApplicationUser user, int n)
    {
        var profile = SeekerProfiles[(n - 1) % SeekerProfiles.Length];
        var city = Cities[(n - 1) % Cities.Length];

        user.Headline = profile.Headline;
        user.Skills = profile.Skills;
        user.City = city;
        user.PhoneNumber = Phone(n);
        user.Website = n % 2 == 0 ? $"https://portfolio{n}.example.com" : null;
        user.About = $"I am a {profile.Headline} based in {city}. I enjoy learning new tools and building useful things. " +
                     "I am looking for a team where I can grow and deliver real work.";
    }

    private static void ApplyEmployerProfile(ApplicationUser user, int n)
    {
        var company = user.CompanyName ?? "Our company";
        var industry = Industries[(n - 1) % Industries.Length];
        var city = EmployerCities[(n - 1) % EmployerCities.Length];
        var slug = new string(company.ToLower().Where(char.IsLetterOrDigit).ToArray());

        user.Industry = industry;
        user.City = city;
        user.PhoneNumber = Phone(n);
        user.Website = $"https://{slug}.example.com";
        user.About = $"{company} is a growing company in the {industry.ToLower()} field, based in {city}. " +
                     "We value honest work, learning and a friendly team. Join us and help build something people use every day.";
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

    // Builds a small valid PDF so the Resume button has a real file to download
    private static byte[] MakeResume(ApplicationUser user)
    {
        var lines = new List<string>
        {
            user.Headline ?? "Job seeker",
            user.City ?? "",
            "",
            "Skills: " + (user.Skills ?? ""),
            "",
            "Sample resume for demo use only."
        };

        var content = new StringBuilder();
        content.Append($"BT /F1 22 Tf 50 780 Td ({Escape(user.FullName)}) Tj ET\n");
        var y = 748;
        foreach (var line in lines)
        {
            content.Append($"BT /F1 12 Tf 50 {y} Td ({Escape(line)}) Tj ET\n");
            y -= 22;
        }
        var stream = content.ToString().TrimEnd('\n');

        var objects = new List<string>
        {
            "<< /Type /Catalog /Pages 2 0 R >>",
            "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >>",
            $"<< /Length {stream.Length} >>\nstream\n{stream}\nendstream",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>"
        };

        var sb = new StringBuilder("%PDF-1.4\n");
        var offsets = new List<int>();
        for (var i = 0; i < objects.Count; i++)
        {
            offsets.Add(sb.Length);
            sb.Append($"{i + 1} 0 obj\n{objects[i]}\nendobj\n");
        }

        var xref = sb.Length;
        sb.Append($"xref\n0 {objects.Count + 1}\n");
        sb.Append("0000000000 65535 f \n");
        foreach (var offset in offsets)
        {
            sb.Append($"{offset:D10} 00000 n \n");
        }
        sb.Append($"trailer\n<< /Size {objects.Count + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF");

        return Encoding.ASCII.GetBytes(sb.ToString());
    }

    private static string Escape(string text)
    {
        return text.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
    }
}