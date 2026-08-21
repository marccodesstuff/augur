using Augur.Domain.Entities;

namespace Augur.Domain.Repositories;

public interface IControlMappingRepository
{
    Task<ControlMapping?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<ControlMapping>> GetBySourceControlAsync(Guid sourceControlId, CancellationToken ct = default);

    Task<IReadOnlyList<ControlMapping>> GetBySourceControlAndTargetFrameworkAsync(
        Guid sourceControlId,
        Augur.Domain.Enums.ComplianceFramework targetFramework,
        CancellationToken ct = default);

    Task<IReadOnlyList<ControlMapping>> GetAllAsync(CancellationToken ct = default);

    Task AddAsync(ControlMapping mapping, CancellationToken ct = default);

    Task UpdateAsync(ControlMapping mapping, CancellationToken ct = default);

    Task DeleteAsync(ControlMapping mapping, CancellationToken ct = default);
}