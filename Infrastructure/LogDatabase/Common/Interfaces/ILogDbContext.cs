using Microsoft.EntityFrameworkCore;
using SharedLibrary.Entities;

namespace Infrastructure.LogDatabase.Common.Interfaces;

public interface ILogDbContext
{
    DbSet<MachineStatusLog> MachineStatusLogs { get; }
    DbSet<MachineMessageLog> MachineMessageLogs { get; }


    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}
