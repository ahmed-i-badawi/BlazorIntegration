using BlazorServer.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorServer.Services;

public interface IApplicationDbContext
{
    DbSet<Brand> Brands { get; }
    DbSet<Site> Sites { get; }
    DbSet<Machine> Machines { get; }
    DbSet<MachineLog> MachineLogs { get; }
    DbSet<Integrator> Integrators { get; }
    DbSet<Zone> Zones { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
