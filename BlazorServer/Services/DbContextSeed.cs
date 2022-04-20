using System.Transactions;
using Infrastructure.ApplicationDatabase.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Entities;

namespace BlazorServer.Data;

public static class DbContextSeed
{
  // ApplicationDBContext
    public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IIdentityService identityService
        )
    {
        var administratorRole = new IdentityRole("Administrator");

        if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await roleManager.CreateAsync(administratorRole);
        }

        var administrator = new ApplicationUser { UserName = "admin", EmailConfirmed = true };

        if (userManager.Users.All(r => r.UserName != administrator.UserName))
        {
            await identityService.CreateUserAsync(administrator.UserName, "12!@qwQW", administrator.EmailConfirmed);
            await userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
        }
    }
    public static async Task SeedSampleDataAsync(IApplicationDbContext context)
    {

        if (!context.Brands?.Any() ?? false)
        {
            var brands = Enumerable.Range(1, 100).Select(x => new Brand()
            {
                Name = $"Brand{x}",
                Notes = $"this is brand{x} Notes"
            });

            await context.Brands.AddRangeAsync(brands);
            await context.SaveChangesAsync();
        }

        if (!context.Sites?.Any() ?? false)
        {
            var Sites = Enumerable.Range(1, 2500).Select(x => new Site()
            {
                Name = $"Site{x}",
                Address = (new string[] { "Cairo", "Giza", "Alex", "USA", "KSA" })[new Random().Next(5)],
                Notes = $"this is Site{x} Notes",
                BrandId = new Random().Next(1, 100),
            });

            await context.Sites.AddRangeAsync(Sites);
            await context.SaveChangesAsync();
        }

        if (!context.Integrators?.Any() ?? false)
        {
            var integrators = Enumerable.Range(1, 500).Select(x => new Integrator()
            {
                Name = $"integrator{x}",
                Notes = $"this is integrator{x} Notes"
            });

            await context.Integrators.AddRangeAsync(integrators);
            await context.SaveChangesAsync();
        }

        if (!context.Zones?.Any() ?? false)
        {
            var zones = Enumerable.Range(1, 100).Select(x => new Zone()
            {
                Name = $"Zone{x}",
                Notes = $"this is Zone{x} Notes",
            });

            await context.Zones.AddRangeAsync(zones);
            await context.SaveChangesAsync();
        }

    }

    // ApplicationDbContext
}
