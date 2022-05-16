using System.Transactions;
using Infrastructure.ApplicationDatabase.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Constants;
using SharedLibrary.Entities;

namespace BlazorServer.Data;

public static class DbContextSeed
{
    // ApplicationDBContext
    public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IIdentityService identityService, IApplicationDbContext context
        )
    {
        // create roles
        if (!roleManager.Roles?.Any() ?? false)
        {
            List<IdentityRole> roles = new List<IdentityRole>()
            {
                new IdentityRole() { Name=RolesConstants.Admin },
                new IdentityRole() { Name=RolesConstants.Site },
            };
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);

            }
        }

        // create users
        if (!userManager.Users?.Any() ?? false)
        {
            //admin user
            var administrator = new ApplicationUser { UserName = "admin", EmailConfirmed = true };

            var user = await identityService.CreateUserAsync(administrator.UserName, "12!@qwQW", administrator.EmailConfirmed);
            await identityService.AddUserToRole(user.UserId, RolesConstants.Admin);

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
    public static async Task SeedSampleDataAsync(IApplicationDbContext context,
        UserManager<ApplicationUser> userManager, IIdentityService identityService)
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

        //var sitesExiat = context.Sites.ToList();
        //var siteUsers = userManager.Users.Where(e=>e.UserName.StartsWith("SiteUser")).Select(e=>e.Id).ToList();

        //int i2 = 1;
        //foreach (var site in sitesExiat)
        //{
        //    var sss = siteUsers[i2];
        //    if (sss != null)
        //    {
        //        site.ApplicationUserId = sss;
        //    }
        //    i2++;
        //    context.SaveChangesAsync();
        //}
        if (!context.Sites?.Any() ?? false)
        {
            List<int> brandids = context.Brands.Select(e => e.Id).ToList();
            List<string> applicationUserIds = userManager.Users.Where(e => e.UserName != "admin").Select(e => e.Id).ToList();

            var ff = brandids.Min();
            var dd = brandids.Max();
            var sites = Enumerable.Range(1, 2500).Select(x => new Site()
            {
                Name = $"Site{x}",
                Address = (new string[] { "Cairo", "Giza", "Alex", "USA", "KSA" })[new Random().Next(5)],
                Notes = $"this is Site{x} Notes",

                BrandId = brandids[new Random().Next(brandids.Count)],
                //BrandId  = new Random().Next(brandids.Min(), brandids.Max()),
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
            try
            {
                await context.SaveChangesAsync();

                // seed sites users
                for (int h = 1; h <= 2500; h++)
                {
                    identityService.CreateUserAsync($"SiteUser{h}", "12!@qwQW", true, $"SiteUser{h}@mail.com");
                }
                var siteUsers = userManager.Users.Where(e => e.UserName.StartsWith("SiteUser")).ToList();
                foreach (var siteUser in siteUsers)
                {
                    identityService.AddUserToRole(siteUser.Id, RolesConstants.Site);
                }
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
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
