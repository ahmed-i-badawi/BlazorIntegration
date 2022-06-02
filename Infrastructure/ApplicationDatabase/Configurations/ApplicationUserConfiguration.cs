using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedLibrary.Entities;

namespace Infrastructure.ApplicationDatabase.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasOne(a => a.Site).WithOne(b => b.ApplicationUser)
        .HasForeignKey<Site>(e => e.ApplicationUserId);

        builder.HasOne(a => a.Integrator).WithOne(b => b.ApplicationUser)
        .HasForeignKey<Integrator>(e => e.ApplicationUserId);
    }
}
