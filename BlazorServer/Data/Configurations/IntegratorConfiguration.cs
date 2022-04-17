using BlazorServer.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class IntegratorConfiguration : IEntityTypeConfiguration<Integrator>
{
    public void Configure(EntityTypeBuilder<Integrator> builder)
    {
        builder.Property(p => p.Id).HasDefaultValueSql("NEWID()");

    }
}
