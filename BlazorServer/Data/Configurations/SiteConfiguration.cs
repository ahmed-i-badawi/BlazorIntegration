using BlazorServer.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.Property(p => p.Hash).HasDefaultValueSql("NEWID()");
        //builder.HasAlternateKey(p => p.Hash);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
    }
}
