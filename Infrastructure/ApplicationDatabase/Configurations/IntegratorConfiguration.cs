using SharedLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.ApplicationDatabase.Configurations;

public class IntegratorConfiguration : IEntityTypeConfiguration<Integrator>
{
    public void Configure(EntityTypeBuilder<Integrator> builder)
    {
        builder.HasKey(p => p.Id);
    }
}
