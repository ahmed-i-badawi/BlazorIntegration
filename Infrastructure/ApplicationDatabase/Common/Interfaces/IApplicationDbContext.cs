using Microsoft.EntityFrameworkCore;
using SharedLibrary.Entities;

namespace Infrastructure.ApplicationDatabase.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Brand> Brands { get; }
    DbSet<Site> Sites { get; }
    DbSet<Machine> Machines { get; }
    DbSet<Integrator> Integrators { get; }
    DbSet<Zone> Zones { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}
