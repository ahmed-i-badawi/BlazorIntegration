using BlazorServer.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Hash).HasDefaultValueSql("NEWID()");

        //builder.Property(m => m.NumberOfZones)
        //    .HasComputedColumnSql(
        //    "(SELECT COUNT(*) FROM [SiteZone] " +
        //    "WHERE [Site].[Id] = [SiteZone].[SiteId]) as PendingRequests"
        //    );

    }
}
