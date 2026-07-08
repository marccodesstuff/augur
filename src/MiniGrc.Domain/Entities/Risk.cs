using MiniGrc.Domain.Common;
using MiniGrc.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniGrc.Domain.Entities;

/// <summary>
/// A risk register entry. Risks may be identified manually or derived by the agent from the set
/// of open findings. Residual severity is computed from likelihood x impact.
/// </summary>
public sealed class Risk : Entity, IAggregateRoot
{
    public string Title { get; private set; }

    public string? Description { get; private set; }

    public int Likelihood { get; private set; }

    public int Impact { get; private set; }

    [NotMapped]
    public RiskSeverity Severity => DeriveSeverity(Likelihood * Impact);

    public bool Accepted { get; private set; }

    private Risk()
    {
        Title = string.Empty;
    }

    private Risk(string title, string? description, int likelihood, int impact)
    {
        Title = title;
        Description = description;
        Likelihood = likelihood;
        Impact = impact;
    }

    public static Risk Create(string title, string? description, int likelihood, int impact)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Risk title is required.", nameof(title));
        if (likelihood is < 1 or > 5) throw new ArgumentOutOfRangeException(nameof(likelihood), "Likelihood must be 1-5.");
        if (impact is < 1 or > 5) throw new ArgumentOutOfRangeException(nameof(impact), "Impact must be 1-5.");

        return new Risk(title.Trim(), description?.Trim(), likelihood, impact);
    }

    public void Accept() { Accepted = true; Touch(); }

    private static RiskSeverity DeriveSeverity(int score) => score switch
    {
        <= 4 => RiskSeverity.Negligible,
        <= 9 => RiskSeverity.Low,
        <= 14 => RiskSeverity.Moderate,
        <= 20 => RiskSeverity.High,
        _ => RiskSeverity.Extreme
    };
}
