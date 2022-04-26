using Infrastructure.LogDatabase;
using Infrastructure.LogDatabase.Common.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogDatabase;

public static class DependencyInjection
{
    public static IServiceCollection AddLogDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<LogDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                b => b.MigrationsAssembly(typeof(LogDbContext).Assembly.FullName)));

        return services;
    }
}
