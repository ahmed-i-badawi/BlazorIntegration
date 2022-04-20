﻿using Infrastructure.ApplicationDatabase.Common;
using Infrastructure.ApplicationDatabase.Common.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Infrastructure.ApplicationDatabase;


public class ApplicationDbContext : IdentityDbContext, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService,
            IDateTime dateTime)
        : base(options)
    {
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<MachineLog> MachineLogs => Set<MachineLog>();
    public DbSet<Integrator> Integrators => Set<Integrator>();
    public DbSet<Zone> Zones => Set<Zone>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _currentUserService.UserId;
                    entry.Entity.Created = _dateTime.Now;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = _currentUserService.UserId;
                    entry.Entity.LastModified = _dateTime.Now;
                    break;
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

}
