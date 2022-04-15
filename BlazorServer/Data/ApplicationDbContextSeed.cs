using System.Transactions;
using BlazorServer.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace BlazorServer.Data;

public static class ApplicationDbContextSeed
{
  
    public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var administratorRole = new IdentityRole("Administrator");

        if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await roleManager.CreateAsync(administratorRole);
        }

        var administrator = new ApplicationUser { UserName = "admin", Email = "admin@localhost" };

        if (userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await userManager.CreateAsync(administrator, "Admin1!");
            await userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
        }
    }

    public static async Task SeedSampleDataAsync(ApplicationDbContext context)
    {

        if (!context.Brands?.Any() ?? false)
        {
            var brands = Enumerable.Range(1, 100).Select(x => new Brand()
            {
                Name = $"Brand{x}",
                Notes = $"this is brand{x} Notes"
            });

            context.Brands.AddRange(brands);
            context.SaveChanges();
        }

        if (!context.Branches?.Any() ?? false)
        {
            var branches = Enumerable.Range(1, 2500).Select(x => new Branch()
            {
                Name = $"Branch{x}",
                Address = (new string[] { "Cairo", "Giza", "Alex", "USA", "KSA" })[new Random().Next(5)],
                Notes = $"this is branch{x} Notes",
                BrandId = new Random().Next(1, 100),
            });

            context.Branches.AddRange(branches);
            context.SaveChanges();
        }

        if (!context.Integrators?.Any() ?? false)
        {
            var integrators = Enumerable.Range(1, 500).Select(x => new Integrator()
            {
                Name = $"integrator{x}",
                Notes = $"this is integrator{x} Notes"
            });

            context.Integrators.AddRange(integrators);
            context.SaveChanges();
        }

    }
}
