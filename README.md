# JobPortal

A full-stack job portal where job seekers find and apply for jobs, employers post jobs and review applicants, and an admin manages the platform.

Built with ASP.NET Core MVC, Entity Framework Core and PostgreSQL. Built for the Auspify internship, Task 5.

**Live demo:** [https://YOURNAME.onrender.com](https://job-portal-3kaa.onrender.com/)

The demo runs on a free plan. If nobody used it for a while, the first page can take up to a minute to open.

## Demo accounts

The login page has a Quick demo login box. Click a card to log in at once.

| Role | Email | Password |
|---|---|---|
| Job seeker | seeker1@demo.com to seeker24@demo.com | Demo@12345 |
| Employer | employer1@demo.com to employer11@demo.com | Demo@12345 |
| Admin | admin@jobportal.com | set by the site owner |

Emails to `@demo.com` addresses are skipped on purpose. To see the status emails, register a job seeker with your own email address.

## Screenshots

Add your screenshots in `docs/screenshots` and link them here.

| Home | Browse jobs | Job details |
|---|---|---|
| ![Home](docs/screenshots/home.png) | ![Jobs](docs/screenshots/jobs.png) | ![Details](docs/screenshots/details.png) |

| Employer applicants | Job seeker dashboard | Admin overview |
|---|---|---|
| ![Applicants](docs/screenshots/applicants.png) | ![Dashboard](docs/screenshots/dashboard.png) | ![Admin](docs/screenshots/admin.png) |

## Features

### Job seeker
- Register and log in as a job seeker
- Browse open jobs with search, location, job type and minimum salary filters
- Sort by newest, oldest, highest salary or title, with pagination
- Apply with a cover note. The current resume is attached to the application
- One application per job. Pending applications can be withdrawn
- Track status as Pending, Shortlisted or Rejected
- Save jobs with a heart button and see them on the Saved page
- Dashboard with application counts and recent applications
- Edit profile: name, phone, city, headline, skills, link and about
- Upload a resume as PDF, DOC or DOCX, up to 2 MB

### Employer
- Register as an employer with a company name
- Post, edit, close, reopen and delete jobs
- Applicants page with status filters
- Shortlist, reject or reset each applicant
- Read cover notes, view the applicant profile and download the resume
- Dashboard with active jobs, applicants and shortlisted counts
- Company profile with industry, website and description, shown on every job page
- Automatic email to the applicant on Shortlist or Reject

### Admin
- Overview of users, jobs and applications
- User management with search, role filter, lock, unlock and delete
- Job moderation with close, reopen and delete

### Platform
- Fully responsive layout
- Glass navbar, scroll animations, toasts and a show password button
- Role-based access for Job seeker, Employer and Admin
- Demo data seeders and a quick demo login box
- Migrations and the admin account are created on startup

## Tech stack

| Area | Technology |
|---|---|
| Backend | ASP.NET Core MVC, C# |
| Data | Entity Framework Core, PostgreSQL with Npgsql |
| Auth | ASP.NET Core Identity with roles |
| Frontend | Razor views, Bootstrap 5, Bootstrap Icons, plain JavaScript |
| Email | MailKit for SMTP, Brevo web API for production |
| Hosting | Docker on Render, Neon PostgreSQL |

## Security

- Passwords are hashed by ASP.NET Core Identity
- Role-based authorization on every protected controller
- Anti-forgery tokens on every form post
- Employers can only see and change their own jobs and applicants
- Resumes can only be downloaded by the owner, the employer who received the application, or an admin
- Resume uploads are checked by extension, size and file signature
- Registration only allows the Job seeker and Employer roles
- Admin can lock accounts. Locked users cannot log in
- Login keys are stored in the database so sessions survive restarts

## Data model

- `ApplicationUser` extends the Identity user with name, company and profile fields
- `Job` belongs to an employer
- `JobApplication` links a job, an applicant, a status, a cover note and a resume. One per applicant and job
- `SavedJob` links a job seeker to a job
- `Resume` stores the file and its details in the database

## Project structure

```
Controllers/   Account, Home, Jobs, Applications, SavedJobs, Profile,
               Resumes, Dashboard, EmployerJobs, Admin
Models/        ApplicationUser, Job, JobApplication, SavedJob, Resume
Data/          ApplicationDbContext, seeders, migrations
Services/      Email service and email templates
ViewModels/    View models for pages
Helpers/       Small UI helpers
Views/         Razor views and shared layout
wwwroot/       CSS, JavaScript and static files
```

## Run locally

You need the .NET SDK that matches `JobPortal.csproj` and a PostgreSQL database.

```bash
git clone https://github.com/Hashim017/JobPortal.git
cd JobPortal
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=HOST;Port=5432;Database=DBNAME;Username=USER;Password=PASSWORD;SSL Mode=Require;Trust Server Certificate=true"
dotnet user-secrets set "Admin:Password" "choose-a-strong-password"
dotnet run
```

The app applies migrations and creates the admin account on startup. The admin email is `admin@jobportal.com` by default.

For demo data, set `"SeedDemoData": true` in `appsettings.json`.

## Configuration

| Setting | Purpose |
|---|---|
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string |
| `Admin:Email` | Admin login email |
| `Admin:Password` | Admin password. The admin is created only when this is set |
| `SeedDemoData` | `true` adds demo users, jobs and applications and shows the quick login box |
| `Email:Enabled` | `true` turns email on |
| `Email:ApiKey` | Brevo API key. When set, email is sent through the web API |
| `Email:FromAddress` | Verified sender address |
| `Email:FromName` | Sender name |
| `Email:Host`, `Email:Port`, `Email:User`, `Email:Password` | SMTP settings for local use |

On the server, use double underscores in names, for example `ConnectionStrings__DefaultConnection`. Never commit passwords or keys.

## Deployment

- The app runs as a Docker container on Render. Every push to `main` deploys by itself
- The database is on Neon PostgreSQL
- Render free web services block SMTP ports, so production email uses the Brevo web API
- A free uptime monitor opens `/health` every few minutes to keep the service awake

## Author

Muhammad Hashim. GitHub: [Hashim017](https://github.com/Hashim017)
