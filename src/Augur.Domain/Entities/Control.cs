using Augur.Domain.Common;
using Augur.Domain.Enums;

namespace Augur.Domain.Entities;

/// <summary>
/// A security or compliance control (e.g. "Access Reviews", "Encryption at Rest") mapped to
/// one or more compliance frameworks. Controls are the heart of the GRC program: evidence is
/// collected against them and their status rolls up into the compliance dashboard.
/// </summary>
public sealed class Control : Entity, IAggregateRoot
{
    public string Code { get; private set; }

    public string Title { get; private set; }

    public string Description { get; private set; }

    public ComplianceFramework Framework { get; private set; }

    public ControlStatus Status { get; private set; }

    public string Owner { get; private set; }

    public IReadOnlyList<Evidence> Evidence => _evidence.AsReadOnly();

    private readonly List<Evidence> _evidence = new();

    private Control()
    {
        Code = string.Empty;
        Title = string.Empty;
        Description = string.Empty;
        Owner = string.Empty;
    }

    private Control(string code, string title, string description, ComplianceFramework framework, string owner)
    {
        Code = code;
        Title = title;
        Description = description;
        Framework = framework;
        Owner = owner;
        Status = ControlStatus.NotImplemented;
    }

    public static Control Create(string code, string title, string description, ComplianceFramework framework, string owner)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Control code is required.", nameof(code));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Control title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(owner)) throw new ArgumentException("Control owner is required.", nameof(owner));

        return new Control(code.Trim(), title.Trim(), description.Trim(), framework, owner.Trim());
    }

    public void Update(string title, string description, string owner)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Control title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(owner)) throw new ArgumentException("Control owner is required.", nameof(owner));

        Title = title.Trim();
        Description = description.Trim();
        Owner = owner.Trim();
        Touch();
        RecomputeStatus();
    }

    public Evidence AttachEvidence(string fileName, string contentType, long sizeBytes, string uploadedBy)
    {
        var evidence = Entities.Evidence.Create(fileName, contentType, sizeBytes, uploadedBy, Id);
        _evidence.Add(evidence);
        Touch();
        RecomputeStatus();
        return evidence;
    }

    public void ReviewEvidence(Guid evidenceId, EvidenceStatus outcome, string? reviewer = null)
    {
        var evidence = _evidence.FirstOrDefault(e => e.Id == evidenceId)
                       ?? throw new InvalidOperationException($"Evidence {evidenceId} not found on control {Id}.");
        evidence.SetStatus(outcome, reviewer);
        Touch();
        RecomputeStatus();
    }

    /// <summary>
    /// Derives <see cref="Status"/> from the evidence attached. Approved evidence implies
    /// Verified; a mix implies Partial; none implies Implemented only if the control was
    /// manually marked Implemented via <see cref="MarkImplemented"/>.
    /// </summary>
    private void RecomputeStatus()
    {
        if (_evidence.Count == 0) return; // keep NotImplemented / manual Implemented
        if (_evidence.Any(e => e.Status == EvidenceStatus.Approved))
            Status = ControlStatus.Verified;
        else if (_evidence.Any(e => e.Status != EvidenceStatus.Rejected))
            Status = ControlStatus.Partial;
    }

    public void MarkImplemented()
    {
        Status = ControlStatus.Implemented;
        Touch();
    }
}
