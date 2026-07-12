using MediatR;
using Microsoft.AspNetCore.Mvc;
using Augur.Application.DTOs;
using Augur.Application.Queries;

namespace Augur.Api.Controllers;

[ApiController]
[Route("api/v1/findings")]
[Produces("application/json")]
public sealed class FindingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FindingsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<FindingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<FindingDto>>> GetAll([FromQuery] bool onlyUnmapped = false)
        => Ok(await _mediator.Send(new GetFindingsQuery(onlyUnmapped)));
}
