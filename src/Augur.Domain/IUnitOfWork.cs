using Augur.Domain.Repositories;

namespace Augur.Domain;

public interface IUnitOfWork
{
    IControlRepository Controls { get; }

    IFindingRepository Findings { get; }

    IRiskRepository Risks { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
