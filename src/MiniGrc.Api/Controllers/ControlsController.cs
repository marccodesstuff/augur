using MediatR;
using Microsoft.AspNetCore.Mvc;
using MiniGrc.Api.Requests;
using MiniGrc.Application.Commands;
using MiniGrc.Application.DTOs;
using MiniGrc.Application.Queries;
using MiniGrc.Domain.Enums;

namespace MiniGrc.Api.Controllers;

[ApiController]
[Route("api/v1/controls")]
[Produces("application/json")]
public sealed class ControlsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ControlsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ControlDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ControlDto>>> GetAll([FromQuery] string? framework)
    {
        ComplianceFramework? filter = ParseFramework(framework);
        var result = await _mediator.Send(new GetControlsQuery(filter));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ControlDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ControlDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetControlByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ControlDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ControlDto>> Create([FromBody] CreateControlRequest request)
    {
        var command = new CreateControlCommand(
            request.Code, request.Title, request.Description, request.Framework, request.Owner);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ControlDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ControlDto>> Update(Guid id, [FromBody] UpdateControlRequest request)
    {
        var result = await _mediator.Send(new UpdateControlCommand(id, request.Title, request.Description, request.Owner));
        return Ok(result);
    }

    [HttpPost("{id:guid}/evidence")]
    [ProducesResponseType(typeof(EvidenceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EvidenceDto>> AttachEvidence(Guid id, [FromBody] AttachEvidenceRequest request)
    {
        var result = await _mediator.Send(new AttachEvidenceCommand(
            id, request.FileName, request.ContentType, request.SizeBytes, request.UploadedBy));
        return Ok(result);
    }

    [HttpPost("{id:guid}/evidence/{evidenceId:guid}/review")]
    [ProducesResponseType(typeof(ControlDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ControlDto>> ReviewEvidence(
        Guid id, Guid evidenceId, [FromBody] ReviewEvidenceRequest request)
    {
        var result = await _mediator.Send(new ReviewEvidenceCommand(id, evidenceId, request.Outcome, request.Reviewer));
        return Ok(result);
    }

    private static ComplianceFramework? ParseFramework(string? framework) => framework?.Trim().ToLowerInvariant() switch
    {
        "soc2" => ComplianceFramework.Soc2,
        "iso27001" => ComplianceFramework.Iso27001,
        null or "" => null,
        _ => throw new ArgumentOutOfRangeException(nameof(framework), "Must be 'Soc2' or 'Iso27001'.")
    };
}
