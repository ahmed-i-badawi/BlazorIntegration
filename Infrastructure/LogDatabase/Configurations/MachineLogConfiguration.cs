using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedLibrary.Entities;

namespace Infrastructure.LogDatabase.Configurations;

public class MachineLogConfiguration : IEntityTypeConfiguration<MachineStatusLog>
{
    public void Configure(EntityTypeBuilder<MachineStatusLog> builder)
    {
        builder.HasKey(p => p.Id);
    }
}
