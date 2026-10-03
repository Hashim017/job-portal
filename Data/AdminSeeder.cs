using JobPortal.Models;
using Microsoft.AspNetCore.Identity;

namespace JobPortal.Data;

public static class AdminSeeder
{
	public static async Task SeedAsync(IServiceProvider services)
	{
		var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
		var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
		var config = services.GetRequiredService<IConfiguration>();

		if (!await roleManager.RoleExistsAsync("Admin"))
		{
			await roleManager.CreateAsync(new IdentityRole("Admin"));
		}

		var email = config["Admin:Email"] ?? "admin@jobportal.com";
		var password = config["Admin:Password"];
		if (string.IsNullOrWhiteSpace(password))
		{
			return;
		}

		if (await userManager.FindByEmailAsync(email) != null)
		{
			return;
		}

		var admin = new ApplicationUser
		{
			UserName = email,
			Email = email,
			FullName = "Administrator",
			EmailConfirmed = true
		};

		var result = await userManager.CreateAsync(admin, password);
		if (result.Succeeded)
		{
			await userManager.AddToRoleAsync(admin, "Admin");
		}
	}
}