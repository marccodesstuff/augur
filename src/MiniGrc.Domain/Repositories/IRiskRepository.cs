using MiniGrc.Domain.Entities;

namespace MiniGrc.Domain.Repositories;

public interface IRiskRepository
{
    Task<IReadOnlyList<Risk>> GetAllAsync(CancellationToken ct = default);

    Task UpsertAsync(Risk risk, CancellationToken ct = default);

    Task DeleteAsync(Risk risk, CancellationToken ct = default);
}
