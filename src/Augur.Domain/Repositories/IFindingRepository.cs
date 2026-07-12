using Augur.Domain.Entities;

namespace Augur.Domain.Repositories;

public interface IFindingRepository
{
    Task<Finding?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<Finding>> GetAllAsync(bool onlyUnmapped = false, CancellationToken ct = default);

    Task<Finding?> GetByExternalIdAsync(string externalId, CancellationToken ct = default);

    Task AddAsync(Finding finding, CancellationToken ct = default);

    Task UpdateAsync(Finding finding, CancellationToken ct = default);
}
