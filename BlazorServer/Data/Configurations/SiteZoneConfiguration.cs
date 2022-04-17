using BlazorServer.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SiteZoneConfiguration : IEntityTypeConfiguration<SiteZone>
{
    public void Configure(EntityTypeBuilder<SiteZone> builder)
    {
        builder.HasKey(p => new { p.SiteId, p.ZoneId });
    }
}
