using MiniGrc.Domain.Enums;

namespace MiniGrc.Agent.Models;

/// <summary>Input to the compliance agent: a policy document or a security-tool export.</summary>
public sealed record AgentRequest(
    string Source,
    string Format,
    string Content,
    ComplianceFramework Framework);

/// <summary>A single finding the agent extracted and mapped.</summary>
public sealed record ExtractedFinding(
    string Title,
    string? Description,
    FindingSeverity Severity,
    string ExternalId,
    string? MappedControlCode,
    IReadOnlyList<RemediationSuggestion> Remediations);

/// <summary>A remediation task the agent suggests for a finding.</summary>
public sealed record RemediationSuggestion(string Title, string? Detail, RemediationPriority Priority);

/// <summary>Aggregated output of a single agent run.</summary>
public sealed record AgentResult(
    IReadOnlyList<ExtractedFinding> Findings,
    int MappedCount,
    string RiskSummary,
    bool UsedLlm,
    long ElapsedMs);
