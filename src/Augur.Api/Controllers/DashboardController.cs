using MediatR;
using Microsoft.AspNetCore.Mvc;
using Augur.Application.DTOs;
using Augur.Application.Queries;

namespace Augur.Api.Controllers;

[ApiController]
[Route("api/v1")]
[Produces("application/json")]
public sealed class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator) => _mediator = mediator;

    [HttpGet("compliance/status")]
    [ProducesResponseType(typeof(ComplianceStatusDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ComplianceStatusDto>> GetComplianceStatus()
        => Ok(await _mediator.Send(new GetComplianceStatusQuery()));

    [HttpGet("risks")]
    [ProducesResponseType(typeof(IReadOnlyList<RiskDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RiskDto>>> GetRisks()
        => Ok(await _mediator.Send(new GetRisksQuery()));
}
