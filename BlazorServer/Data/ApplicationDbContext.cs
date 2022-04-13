using BlazorServer.Data.Entities;
using BlazorServer.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorServer.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Branch> Branchs => Set<Branch>();
        public DbSet<Machine> Machines => Set<Machine>();
        public DbSet<MachineLog> MachineLogs => Set<MachineLog>();
        public DbSet<Integrator> Integrators => Set<Integrator>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Integrator>().Property(p=>p.Id).HasDefaultValueSql("NEWID()");

            base.OnModelCreating(modelBuilder);
        }

    }
}