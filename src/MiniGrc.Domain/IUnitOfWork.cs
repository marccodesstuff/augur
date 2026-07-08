using MiniGrc.Domain.Repositories;

namespace MiniGrc.Domain;

public interface IUnitOfWork
{
    IControlRepository Controls { get; }

    IFindingRepository Findings { get; }

    IRiskRepository Risks { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
