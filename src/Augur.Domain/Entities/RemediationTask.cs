using Augur.Domain.Common;
using Augur.Domain.Enums;

namespace Augur.Domain.Entities;

/// <summary>
/// A remediation task drafted by the AI agent to close a <see cref="Finding"/>. Child entity of
/// <see cref="Finding"/>.
/// </summary>
public sealed class RemediationTask : Entity
{
    public string Title { get; private set; }

    public string? Detail { get; private set; }

    public RemediationPriority Priority { get; private set; }

    public Guid FindingId { get; private set; }

    private RemediationTask()
    {
        Title = string.Empty;
    }

    private RemediationTask(string title, string? detail, RemediationPriority priority, Guid findingId)
    {
        Title = title;
        Detail = detail;
        Priority = priority;
        FindingId = findingId;
    }

    public static RemediationTask Create(string title, string? detail, RemediationPriority priority, Guid findingId)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Task title is required.", nameof(title));
        return new RemediationTask(title.Trim(), detail?.Trim(), priority, findingId);
    }
}
