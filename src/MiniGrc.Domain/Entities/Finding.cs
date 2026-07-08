using MiniGrc.Domain.Common;
using MiniGrc.Domain.Enums;

namespace MiniGrc.Domain.Entities;

/// <summary>
/// A security finding surfaced by a tool (e.g. GitHub Dependabot, a vulnerability scanner) or
/// by a policy review. The AI agent maps findings onto <see cref="Control"/>s and drafts
/// remediation tasks. Findings are produced by the agent pipeline and stored for audit.
/// </summary>
public sealed class Finding : Entity, IAggregateRoot
{
    public string Title { get; private set; }

    public string? Description { get; private set; }

    public FindingSeverity Severity { get; private set; }

    public string Source { get; private set; }

    public string ExternalId { get; private set; }

    public Guid? MappedControlId { get; private set; }

    public string? SuggestedControlCode { get; private set; }

    public bool Mapped { get; private set; }

    public IReadOnlyList<RemediationTask> RemediationTasks => _remediationTasks.AsReadOnly();

    private readonly List<RemediationTask> _remediationTasks = new();

    private Finding()
    {
        Title = string.Empty;
        Source = string.Empty;
        ExternalId = string.Empty;
    }

    private Finding(string title, string? description, FindingSeverity severity, string source, string externalId)
    {
        Title = title;
        Description = description;
        Severity = severity;
        Source = source;
        ExternalId = externalId;
    }

    public static Finding Create(string title, string? description, FindingSeverity severity, string source, string externalId)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Finding title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(source)) throw new ArgumentException("Finding source is required.", nameof(source));

        return new Finding(title.Trim(), description?.Trim(), severity, source.Trim(), externalId.Trim());
    }

    public void ApplyMapping(Guid? mappedControlId, string? suggestedControlCode)
    {
        MappedControlId = mappedControlId;
        SuggestedControlCode = suggestedControlCode?.Trim();
        Mapped = mappedControlId.HasValue || !string.IsNullOrWhiteSpace(suggestedControlCode);
        Touch();
    }

    public RemediationTask AddRemediationTask(string title, string? detail, RemediationPriority priority)
    {
        var task = RemediationTask.Create(title, detail, priority, Id);
        _remediationTasks.Add(task);
        Touch();
        return task;
    }
}
