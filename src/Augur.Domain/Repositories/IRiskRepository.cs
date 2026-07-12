using Augur.Domain.Entities;

namespace Augur.Domain.Repositories;

public interface IRiskRepository
{
    Task<IReadOnlyList<Risk>> GetAllAsync(CancellationToken ct = default);

    Task UpsertAsync(Risk risk, CancellationToken ct = default);

    Task DeleteAsync(Risk risk, CancellationToken ct = default);
}
