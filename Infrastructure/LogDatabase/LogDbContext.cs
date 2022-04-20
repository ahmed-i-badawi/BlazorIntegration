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
    public DbSet<TestLog> TestLogs => Set<TestLog>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }


}
