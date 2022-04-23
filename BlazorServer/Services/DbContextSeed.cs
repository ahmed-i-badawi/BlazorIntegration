using System.Transactions;
using Infrastructure.ApplicationDatabase.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        // create roles

        if (!roleManager.Roles?.Any() ?? false)
        {
            List<IdentityRole> roles = new List<IdentityRole>()
            {
                new IdentityRole() { Name="ADMINISTRATOR" },
                new IdentityRole() { Name="SITE" },
            };
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);

            }
        }


        // create users
        if (!userManager.Users?.Any() ?? false)
        {
            // admin user
            var administrator = new ApplicationUser { UserName = "admin", EmailConfirmed = true };

            await identityService.CreateUserAsync(administrator.UserName, "12!@qwQW", administrator.EmailConfirmed);
            await userManager.AddToRolesAsync(administrator, new[] { "ADMINISTRATOR" });

            // --------

            ////site users
            //var sitesUsers = Enumerable.Range(1, 10).Select(x => new ApplicationUser()
            //{
            //    UserName = $"SiteUser{x}",
            //    EmailConfirmed = true,
            //}).ToList();
            //string defaultPassword = "12!@qwQW";

            //foreach (var sitesUser in sitesUsers)
            //{
            //    await userManager.CreateAsync(sitesUser, defaultPassword);
            //    //identityService.CreateUserAsync(sitesUser.UserName, defaultPassword, sitesUser.EmailConfirmed);
            //    //userManager.AddToRolesAsync(sitesUser, new[] { "SITE" });
            //}
            ////var applicationUsers = await userManager.Users.Where(e => e.UserName != "admin").ToListAsync();
            ////foreach (var applicationUser in applicationUsers)
            ////{
            ////}
        }
    }
    public static async Task SeedSampleDataAsync(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
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
            List<int> brandids = context.Brands.Select(e => e.Id).ToList();
            List<string> applicationUserIds = userManager.Users.Where(e => e.UserName != "admin").Select(e => e.Id).ToList();

            var sites = Enumerable.Range(1, 2500).Select(x => new Site()
            {
                Name = $"Site{x}",
                Address = (new string[] { "Cairo", "Giza", "Alex", "USA", "KSA" })[new Random().Next(5)],
                Notes = $"this is Site{x} Notes",
                BrandId = new Random().Next(brandids.Min(), brandids.Max()),
            }); ;

            int i = 1;
            foreach (var userId in applicationUserIds)
            {
                var siteItem = sites.FirstOrDefault(e => e.Id == i);
                if (siteItem != null)
                {
                    siteItem.ApplicationUserId = userId;
                }
                i++;
            }

            await context.Sites.AddRangeAsync(sites);
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
