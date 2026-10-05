<div align="center">

# 💼 Job Portal

**Connecting job seekers with employers.**

![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core_MVC-512BD4?logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?logo=docker&logoColor=white)
![Render](https://img.shields.io/badge/Render-46E3B7?logo=render&logoColor=black)

[Live Demo](https://job-portal-3kaa.onrender.com/)

</div>

## 📑 Table of Contents

- [About](#-about)
- [Features by Role](#-features-by-role)
- [Tech Stack](#-tech-stack)
- [Screenshots](#-screenshots)
- [Getting Started](#-getting-started)
- [Demo Accounts](#-demo-accounts)
- [Author](#-author)

## 📖 About

Job Portal is a hiring platform with three roles. Job seekers find and apply for jobs. Employers post jobs and manage applicants. Admins keep the platform clean. It was built as Task 5 of the Auspify internship.

## 🚀 Features by Role

| Role | Features |
|---|---|
| Job Seeker | Search and filter jobs, apply, save jobs, upload a resume, edit profile, get an email when an application status changes |
| Employer | Post and manage jobs, review applications, update status, edit company profile |
| Admin | Manage users, moderate job posts, view the admin dashboard |

Also included: sorting, pagination and a demo data seeder.

## 🧰 Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC on .NET 9 |
| Language | C# |
| Database | PostgreSQL on Neon |
| ORM | Entity Framework Core with Npgsql |
| Hosting | Docker on Render |

## 🖼 Screenshots

### Landing Page

#### Landing Page
<img src="docs/screenshots/landing-page.PNG" alt="Landing Page" width="600">

#### Landing Page - Section 2
<img src="docs/screenshots/landing-page2.PNG" alt="Landing Page 2" width="600">


### Job Browsing

#### Browse Jobs
<img src="docs/screenshots/browse-jobs-page.PNG" alt="Browse Jobs" width="600">

#### Browse Jobs with Filters
<img src="docs/screenshots/browse-jobs-page-with-filters.PNG" alt="Browse Jobs with Filters" width="600">

#### Job Details
<img src="docs/screenshots/job-details-page.PNG" alt="Job Details" width="600">


### Job Seeker

#### Job Seeker Dashboard
<img src="docs/screenshots/jobseeker-dashboard.PNG" alt="Job Seeker Dashboard" width="600">

#### My Applications
<img src="docs/screenshots/my-applications-page.PNG" alt="My Applications" width="600">

#### Saved Jobs
<img src="docs/screenshots/save-jobs-page.PNG" alt="Saved Jobs" width="600">

#### Edit Account Details
<img src="docs/screenshots/user-profile-edit-page.PNG" alt="Edit Account Details" width="600">

#### Edit Account Details - Additional View
<img src="docs/screenshots/user-profile-edit-page2.PNG" alt="Edit Account Details 2" width="600">


### Employer

#### Employer Dashboard
<img src="docs/screenshots/employer-dashboard.PNG" alt="Employer Dashboard" width="600">

#### Employer Dashboard - Additional View
<img src="docs/screenshots/employer-dashboard2.PNG" alt="Employer Dashboard 2" width="600">

#### Employer Jobs
<img src="docs/screenshots/employer-jobs-page.PNG" alt="Employer Jobs" width="600">

#### Edit Job
<img src="docs/screenshots/edit-job-page.PNG" alt="Edit Job" width="600">


### Authentication

#### Register / Login
<img src="docs/screenshots/register-login-page.PNG" alt="Register Login" width="600">


### Other

#### Working & Footer
<img src="docs/screenshots/working-and-footer.PNG" alt="Working and Footer" width="600">


## Responsive Design

The application is fully responsive and optimized for desktop, tablet, and mobile devices.

### Mobile Landing Page

<img src="docs/screenshots/landing-page-mobile.jpg" alt="Landing Page Mobile" width="300">

<img src="docs/screenshots/landing-page2-mobile.jpg" alt="Landing Page Mobile 2" width="300">

<img src="docs/screenshots/landing-page3-mobile.jpg" alt="Landing Page Mobile 3" width="300">


### Mobile Job Browsing

<img src="docs/screenshots/browse-jobs-page-mobile.jpg" alt="Browse Jobs Mobile" width="300">

<img src="docs/screenshots/browse-jobs-page-with-filters-mobile.jpg" alt="Browse Jobs with Filters Mobile" width="300">


### Mobile Job Details

<img src="docs/screenshots/job-details-mobile.jpg" alt="Job Details Mobile" width="300">


### Mobile Job Seeker

<img src="docs/screenshots/jobseeker-dashboard-mobile.jpg" alt="Job Seeker Dashboard Mobile" width="300">

<img src="docs/screenshots/jobseeker-applications-mobile.jpg" alt="Jobseeker Applications Mobile" width="300">

<img src="docs/screenshots/account-details-update-page-mobile.jpg" alt="Account Details Update Mobile" width="300">

<img src="docs/screenshots/account-details-update-page2-mobile.jpg" alt="Account Details Update Mobile 2" width="300">


### Mobile Employer

<img src="docs/screenshots/employer-dashboard-mobile.jpg" alt="Employer Dashboard Mobile" width="300">

<img src="docs/screenshots/employer-jobs-page-mobile.jpg" alt="Employer Jobs Mobile" width="300">


### Mobile Footer

<img src="docs/screenshots/footer-mobile.jpg" alt="Footer Mobile" width="300">

## ⚙️ Getting Started

**You need:** .NET SDK 9 and a PostgreSQL database.

```bash
git clone https://github.com/Hashim017/job-portal.git
cd job-portal
```

Open `appsettings.Development.json` in the project root and set your database link:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-postgres-connection-string"
  }
}
```

Run the app:

```bash
dotnet restore
dotnet run
```

## 🔑 Demo Accounts

| Role | Email | Password |
|---|---|---|
| Admin | `admin@jobportal.com` | `Admin@12345` |
| Employer | `employer1@demo.com` | `Demo@12345` |
| Job Seeker | `seeker1@demo.com` | `Demo@12345` |

## 👤 Author

**Muhammad Hashim** - [GitHub](https://github.com/Hashim017)