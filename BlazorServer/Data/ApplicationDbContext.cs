using BlazorServer.Data.Entities;
using BlazorServer.Extensions;
using BlazorServer.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace BlazorServer.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<MachineLog> MachineLogs => Set<MachineLog>();
    public DbSet<Integrator> Integrators => Set<Integrator>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

}
