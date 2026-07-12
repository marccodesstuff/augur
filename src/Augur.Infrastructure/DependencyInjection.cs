using Microsoft.EntityFrameworkCore;
using Augur.Domain;
using Augur.Infrastructure.Persistence;
using Augur.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Augur.Infrastructure;

/// <summary>
/// Composition root for the Infrastructure layer. Wires the EF Core DbContext to PostgreSQL and
/// registers the concrete <c>IUnitOfWork</c> implementation behind the Domain port.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Adds the Infrastructure layer services to the container.</summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">PostgreSQL connection string.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AugurDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly(typeof(AugurDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
