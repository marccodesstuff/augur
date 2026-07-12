using Augur.Domain.Common;
using Augur.Domain.Enums;

namespace Augur.Domain.Entities;

/// <summary>
/// An evidence artifact (a policy PDF, a scanner export, a screenshot) uploaded to prove a
/// control operates. Child entity of <see cref="Control"/>; never persisted on its own.
/// </summary>
public sealed class Evidence : Entity
{
    public string FileName { get; private set; }

    public string ContentType { get; private set; }

    public long SizeBytes { get; private set; }

    public string UploadedBy { get; private set; }

    public EvidenceStatus Status { get; private set; }

    public string? Reviewer { get; private set; }

    public Guid ControlId { get; private set; }

    private Evidence()
    {
        FileName = string.Empty;
        ContentType = string.Empty;
        UploadedBy = string.Empty;
    }

    private Evidence(string fileName, string contentType, long sizeBytes, string uploadedBy, Guid controlId)
    {
        FileName = fileName;
        ContentType = contentType;
        SizeBytes = sizeBytes;
        UploadedBy = uploadedBy;
        ControlId = controlId;
        Status = EvidenceStatus.PendingReview;
    }

    public static Evidence Create(string fileName, string contentType, long sizeBytes, string uploadedBy, Guid controlId)
    {
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("File name is required.", nameof(fileName));
        if (sizeBytes <= 0) throw new ArgumentOutOfRangeException(nameof(sizeBytes), "Size must be positive.");

        return new Evidence(fileName.Trim(), contentType.Trim(), sizeBytes, uploadedBy.Trim(), controlId);
    }

    public void SetStatus(EvidenceStatus status, string? reviewer = null)
    {
        Status = status;
        Reviewer = reviewer?.Trim();
        Touch();
    }
}
