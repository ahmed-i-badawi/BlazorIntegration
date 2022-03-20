using BlazorServer.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorServer.Services;

public interface IApplicationDbContext
{
    DbSet<Connection> Connections { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
