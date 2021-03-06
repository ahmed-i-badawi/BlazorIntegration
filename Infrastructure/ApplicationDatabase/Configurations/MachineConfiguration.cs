using SharedLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.ApplicationDatabase.Configurations;

public class MachineConfiguration : IEntityTypeConfiguration<Machine>
{
    public void Configure(EntityTypeBuilder<Machine> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(e => e.FingerPrint).IsUnique();

    }
}
