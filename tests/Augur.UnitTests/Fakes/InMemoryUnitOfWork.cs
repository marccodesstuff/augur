using Augur.Domain;
using Augur.Domain.Entities;
using Augur.Domain.Enums;
using Augur.Domain.Repositories;

namespace Augur.UnitTests.Fakes;

public sealed class InMemoryUnitOfWork : IUnitOfWork
{
    private readonly List<Control> _controls = new();
    private readonly List<ControlMapping> _controlMappings = new();
    private readonly List<Finding> _findings = new();
    private readonly List<Risk> _risks = new();

    public IControlRepository Controls { get; }

    public IControlMappingRepository ControlMappings { get; }

    public IFindingRepository Findings { get; }

    public IRiskRepository Risks { get; }

    public InMemoryUnitOfWork()
    {
        Controls = new InMemoryControlRepository(_controls);
        ControlMappings = new InMemoryControlMappingRepository(_controlMappings);
        Findings = new InMemoryFindingRepository(_findings);
        Risks = new InMemoryRiskRepository(_risks);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        // No-op: the in-memory collections are mutated directly by the repositories.
        return Task.FromResult(1);
    }

    private sealed class InMemoryControlRepository : IControlRepository
    {
        private readonly List<Control> _store;
        public InMemoryControlRepository(List<Control> store) => _store = store;

        public Task<Control?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult(_store.FirstOrDefault(c => c.Id == id));

        public Task<IReadOnlyList<Control>> GetAllAsync(ComplianceFramework? framework = null, CancellationToken ct = default)
        {
            var q = framework.HasValue ? _store.Where(c => c.Framework == framework.Value) : _store;
            return Task.FromResult<IReadOnlyList<Control>>(q.OrderBy(c => c.Code).ToList());
        }

        public Task<Control?> GetByCodeAsync(string code, CancellationToken ct = default)
            => Task.FromResult(_store.FirstOrDefault(c => c.Code == code));

        public Task AddAsync(Control control, CancellationToken ct = default)
        {
            _store.Add(control);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Control control, CancellationToken ct = default)
        {
            var idx = _store.FindIndex(c => c.Id == control.Id);
            if (idx >= 0) _store[idx] = control;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Control control, CancellationToken ct = default)
        {
            _store.RemoveAll(c => c.Id == control.Id);
            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryControlMappingRepository : IControlMappingRepository
    {
        private readonly List<ControlMapping> _store;
        public InMemoryControlMappingRepository(List<ControlMapping> store) => _store = store;

        public Task<ControlMapping?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult(_store.FirstOrDefault(m => m.Id == id));

        public Task<IReadOnlyList<ControlMapping>> GetBySourceControlAsync(Guid sourceControlId, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<ControlMapping>>(_store.Where(m => m.SourceControlId == sourceControlId).OrderBy(m => m.TargetFramework).ThenBy(m => m.TargetControlCode).ToList());

        public Task<IReadOnlyList<ControlMapping>> GetBySourceControlAndTargetFrameworkAsync(Guid sourceControlId, ComplianceFramework targetFramework, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<ControlMapping>>(_store.Where(m => m.SourceControlId == sourceControlId && m.TargetFramework == targetFramework).OrderBy(m => m.TargetControlCode).ToList());

        public Task<IReadOnlyList<ControlMapping>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<ControlMapping>>(_store.OrderBy(m => m.SourceFramework).ThenBy(m => m.SourceControlCode).ThenBy(m => m.TargetFramework).ToList());

        public Task AddAsync(ControlMapping mapping, CancellationToken ct = default)
        {
            _store.Add(mapping);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(ControlMapping mapping, CancellationToken ct = default)
        {
            var idx = _store.FindIndex(m => m.Id == mapping.Id);
            if (idx >= 0) _store[idx] = mapping;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(ControlMapping mapping, CancellationToken ct = default)
        {
            _store.RemoveAll(m => m.Id == mapping.Id);
            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryFindingRepository : IFindingRepository
    {
        private readonly List<Finding> _store;
        public InMemoryFindingRepository(List<Finding> store) => _store = store;

        public Task<Finding?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult(_store.FirstOrDefault(f => f.Id == id));

        public Task<IReadOnlyList<Finding>> GetAllAsync(bool onlyUnmapped = false, CancellationToken ct = default)
        {
            var q = onlyUnmapped ? _store.Where(f => !f.Mapped) : _store;
            return Task.FromResult<IReadOnlyList<Finding>>(q.OrderByDescending(f => f.CreatedAtUtc).ToList());
        }

        public Task<Finding?> GetByExternalIdAsync(string externalId, CancellationToken ct = default)
            => Task.FromResult(_store.FirstOrDefault(f => f.ExternalId == externalId));

        public Task AddAsync(Finding finding, CancellationToken ct = default)
        {
            _store.Add(finding);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Finding finding, CancellationToken ct = default)
        {
            var idx = _store.FindIndex(f => f.Id == finding.Id);
            if (idx >= 0) _store[idx] = finding;
            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryRiskRepository : IRiskRepository
    {
        private readonly List<Risk> _store;
        public InMemoryRiskRepository(List<Risk> store) => _store = store;

        public Task<IReadOnlyList<Risk>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<Risk>>(_store.OrderByDescending(r => r.Likelihood * r.Impact).ToList());

        public Task UpsertAsync(Risk risk, CancellationToken ct = default)
        {
            var idx = _store.FindIndex(r => r.Id == risk.Id);
            if (idx >= 0) _store[idx] = risk; else _store.Add(risk);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Risk risk, CancellationToken ct = default)
        {
            _store.RemoveAll(r => r.Id == risk.Id);
            return Task.CompletedTask;
        }
    }
}
