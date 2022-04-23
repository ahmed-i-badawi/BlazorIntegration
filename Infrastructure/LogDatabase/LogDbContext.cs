using Infrastructure.LogDatabase.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Infrastructure.LogDatabase;

public class LogDbContext : DbContext, ILogDbContext
{
    public LogDbContext(DbContextOptions<LogDbContext> options)
        : base(options)
    {
    }
    public DbSet<MachineStatusLog> MachineStatusLogs => Set<MachineStatusLog>();
    public DbSet<MachineMessageLog> MachineMessageLogs => Set<MachineMessageLog>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        //foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        //{
        //    switch (entry.State)
        //    {
        //        case EntityState.Added:
        //            entry.Entity.CreatedBy = _currentUserService.UserId;
        //            entry.Entity.Created = _dateTime.Now;
        //            break;

        //        case EntityState.Modified:
        //            entry.Entity.LastModifiedBy = _currentUserService.UserId;
        //            entry.Entity.LastModified = _dateTime.Now;
        //            break;
        //    }
        //}

        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }


}
