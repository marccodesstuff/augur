using MediatR;
using MiniGrc.Application.DTOs;
using MiniGrc.Domain.Enums;

namespace MiniGrc.Application.Queries;

public sealed record GetControlsQuery(ComplianceFramework? Framework = null) : IRequest<IReadOnlyList<ControlDto>>;

public sealed record GetControlByIdQuery(Guid Id) : IRequest<ControlDto?>;

public sealed record GetComplianceStatusQuery : IRequest<ComplianceStatusDto>;

public sealed record GetFindingsQuery(bool OnlyUnmapped = false) : IRequest<IReadOnlyList<FindingDto>>;

public sealed record GetRisksQuery : IRequest<IReadOnlyList<RiskDto>>;
