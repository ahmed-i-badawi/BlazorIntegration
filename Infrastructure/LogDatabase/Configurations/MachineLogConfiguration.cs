using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedLibrary.Entities;

namespace Infrastructure.LogDatabase.Configurations;

public class MachineLogConfiguration : IEntityTypeConfiguration<MachineLog>
{
    public void Configure(EntityTypeBuilder<MachineLog> builder)
    {
        builder.HasKey(p => p.Id);
    }
}
