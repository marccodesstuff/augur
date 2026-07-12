using Microsoft.EntityFrameworkCore;
using Augur.Domain;
using Augur.Domain.Entities;
using Augur.Domain.Enums;
using Augur.Domain.Repositories;
using Augur.Infrastructure.Persistence;

namespace Augur.Infrastructure.Repositories;

public sealed class ControlRepository : IControlRepository
{
    private readonly AugurDbContext _db;

    public ControlRepository(AugurDbContext db) => _db = db;

    public Task<Control?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Controls.Include(c => c.Evidence).FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Control>> GetAllAsync(ComplianceFramework? framework = null, CancellationToken ct = default)
    {
        var query = _db.Controls.Include(c => c.Evidence).AsQueryable();
        if (framework.HasValue) query = query.Where(c => c.Framework == framework.Value);
        return await query.OrderBy(c => c.Code).ToListAsync(ct);
    }

    public Task<Control?> GetByCodeAsync(string code, CancellationToken ct = default)
        => _db.Controls.FirstOrDefaultAsync(c => c.Code == code, ct);

    public async Task AddAsync(Control control, CancellationToken ct = default)
    {
        await _db.Controls.AddAsync(control, ct);
    }

    public Task UpdateAsync(Control control, CancellationToken ct = default)
    {
        _db.Controls.Update(control);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Control control, CancellationToken ct = default)
    {
        _db.Controls.Remove(control);
        await Task.CompletedTask;
    }
}

public sealed class FindingRepository : IFindingRepository
{
    private readonly AugurDbContext _db;

    public FindingRepository(AugurDbContext db) => _db = db;

    public Task<Finding?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Findings.Include(f => f.RemediationTasks).FirstOrDefaultAsync(f => f.Id == id, ct);

    public async Task<IReadOnlyList<Finding>> GetAllAsync(bool onlyUnmapped = false, CancellationToken ct = default)
    {
        var query = _db.Findings.Include(f => f.RemediationTasks).AsQueryable();
        if (onlyUnmapped) query = query.Where(f => !f.Mapped);
        return await query.OrderByDescending(f => f.CreatedAtUtc).ToListAsync(ct);
    }

    public Task<Finding?> GetByExternalIdAsync(string externalId, CancellationToken ct = default)
        => _db.Findings.FirstOrDefaultAsync(f => f.ExternalId == externalId, ct);

    public async Task AddAsync(Finding finding, CancellationToken ct = default)
    {
        await _db.Findings.AddAsync(finding, ct);
    }

    public Task UpdateAsync(Finding finding, CancellationToken ct = default)
    {
        _db.Findings.Update(finding);
        return Task.CompletedTask;
    }
}

public sealed class RiskRepository : IRiskRepository
{
    private readonly AugurDbContext _db;

    public RiskRepository(AugurDbContext db) => _db = db;

    public async Task<IReadOnlyList<Risk>> GetAllAsync(CancellationToken ct = default)
        => await _db.Risks.OrderByDescending(r => r.Likelihood * r.Impact).ToListAsync(ct);

    public async Task UpsertAsync(Risk risk, CancellationToken ct = default)
    {
        var existing = await _db.Risks.FindAsync([risk.Id], ct);
        if (existing is null) await _db.Risks.AddAsync(risk, ct);
        else _db.Risks.Update(risk);
    }

    public Task DeleteAsync(Risk risk, CancellationToken ct = default)
    {
        _db.Risks.Remove(risk);
        return Task.CompletedTask;
    }
}

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AugurDbContext _db;

    public UnitOfWork(AugurDbContext db)
    {
        _db = db;
        Controls = new ControlRepository(db);
        Findings = new FindingRepository(db);
        Risks = new RiskRepository(db);
    }

    public IControlRepository Controls { get; }

    public IFindingRepository Findings { get; }

    public IRiskRepository Risks { get; }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
