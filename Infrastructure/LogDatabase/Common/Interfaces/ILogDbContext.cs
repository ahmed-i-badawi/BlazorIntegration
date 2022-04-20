using Microsoft.EntityFrameworkCore;
using SharedLibrary.Entities;

namespace Infrastructure.LogDatabase.Common.Interfaces;

public interface ILogDbContext
{
    DbSet<MachineLog> MachineLogs { get; }


    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}
