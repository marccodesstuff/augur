using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Augur.Infrastructure.Persistence;

namespace Augur.Infrastructure.Persistence;

/// <summary>
/// Design-time factory used by <c>dotnet ef migrations</c> to construct <see cref="AugurDbContext"/>
/// without booting the full application host. Uses a local Postgres connection by default; the
/// connection string can be overridden with the <c>ConnectionStrings__Augur</c> environment variable.
/// </summary>
public sealed class AugurDbContextFactory : IDesignTimeDbContextFactory<AugurDbContext>
{
    /// <inheritdoc/>
    public AugurDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Augur")
            ?? "Host=localhost;Port=5432;Database=augur;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<AugurDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new AugurDbContext(optionsBuilder.Options);
    }
}
