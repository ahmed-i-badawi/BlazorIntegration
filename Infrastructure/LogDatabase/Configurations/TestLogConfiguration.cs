using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedLibrary.Entities;

namespace Infrastructure.LogDatabase.Configurations;

public class TestLogConfiguration : IEntityTypeConfiguration<TestLog>
{
    public void Configure(EntityTypeBuilder<TestLog> builder)
    {
        builder.HasKey(p => p.Id);
    }
}
