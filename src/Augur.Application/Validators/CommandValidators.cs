using FluentValidation;
using Augur.Application.Commands;

namespace Augur.Application.Validators;

/// <summary>Validates <see cref="CreateControlCommand"/>. Fails fast before the handler runs.</summary>
public sealed class CreateControlCommandValidator : AbstractValidator<CreateControlCommand>
{
    /// <summary>Configures the rules.</summary>
    public CreateControlCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Owner).NotEmpty().MaximumLength(200);
    }
}

/// <summary>Validates <see cref="UpdateControlCommand"/>.</summary>
public sealed class UpdateControlCommandValidator : AbstractValidator<UpdateControlCommand>
{
    /// <summary>Configures the rules.</summary>
    public UpdateControlCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Owner).NotEmpty().MaximumLength(200);
    }
}

/// <summary>Validates <see cref="AttachEvidenceCommand"/>.</summary>
public sealed class AttachEvidenceCommandValidator : AbstractValidator<AttachEvidenceCommand>
{
    /// <summary>Configures the rules.</summary>
    public AttachEvidenceCommandValidator()
    {
        RuleFor(x => x.ControlId).NotEmpty();
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x.SizeBytes).GreaterThan(0);
        RuleFor(x => x.UploadedBy).NotEmpty();
    }
}

/// <summary>Validates <see cref="ReviewEvidenceCommand"/>.</summary>
public sealed class ReviewEvidenceCommandValidator : AbstractValidator<ReviewEvidenceCommand>
{
    /// <summary>Configures the rules.</summary>
    public ReviewEvidenceCommandValidator()
    {
        RuleFor(x => x.ControlId).NotEmpty();
        RuleFor(x => x.EvidenceId).NotEmpty();
    }
}

/// <summary>Validates <see cref="CreateRiskCommand"/>.</summary>
public sealed class CreateRiskCommandValidator : AbstractValidator<CreateRiskCommand>
{
    /// <summary>Configures the rules.</summary>
    public CreateRiskCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Likelihood).InclusiveBetween(1, 5);
        RuleFor(x => x.Impact).InclusiveBetween(1, 5);
    }
}

/// <summary>Validates <see cref="CreateControlMappingCommand"/>.</summary>
public sealed class CreateControlMappingCommandValidator : AbstractValidator<CreateControlMappingCommand>
{
    /// <summary>Configures the rules.</summary>
    public CreateControlMappingCommandValidator()
    {
        RuleFor(x => x.SourceControlId).NotEmpty();
        RuleFor(x => x.SourceControlCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TargetControlCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TargetControlTitle).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ConfidenceScore).InclusiveBetween(0, 100);
        RuleFor(x => x.Rationale).NotEmpty().MaximumLength(1000);
    }
}

/// <summary>Validates <see cref="UpdateControlMappingCommand"/>.</summary>
public sealed class UpdateControlMappingCommandValidator : AbstractValidator<UpdateControlMappingCommand>
{
    /// <summary>Configures the rules.</summary>
    public UpdateControlMappingCommandValidator()
    {
        RuleFor(x => x.MappingId).NotEmpty();
        RuleFor(x => x.ConfidenceScore).InclusiveBetween(0, 100);
        RuleFor(x => x.Rationale).NotEmpty().MaximumLength(1000);
    }
}

/// <summary>Validates <see cref="DeleteControlMappingCommand"/>.</summary>
public sealed class DeleteControlMappingCommandValidator : AbstractValidator<DeleteControlMappingCommand>
{
    /// <summary>Configures the rules.</summary>
    public DeleteControlMappingCommandValidator()
    {
        RuleFor(x => x.MappingId).NotEmpty();
    }
}
