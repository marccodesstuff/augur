using Augur.Domain.Entities;
using Augur.Domain.Enums;

namespace Augur.Domain.Repositories;

public interface IControlRepository
{
    Task<Control?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<Control>> GetAllAsync(ComplianceFramework? framework = null, CancellationToken ct = default);

    Task<Control?> GetByCodeAsync(string code, CancellationToken ct = default);

    Task AddAsync(Control control, CancellationToken ct = default);

    Task UpdateAsync(Control control, CancellationToken ct = default);

    Task DeleteAsync(Control control, CancellationToken ct = default);
}
