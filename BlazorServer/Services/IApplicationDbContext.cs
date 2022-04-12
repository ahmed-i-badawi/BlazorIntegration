using BlazorServer.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorServer.Services;

public interface IApplicationDbContext
{
    DbSet<Brand> Brands { get; }
    DbSet<Branch> Branchs { get; }
    DbSet<Machine> Machines { get; }
    DbSet<MachineLog> MachineLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
