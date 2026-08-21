using MediatR;
using Microsoft.AspNetCore.Mvc;
using Augur.Api.Requests;
using Augur.Application.Commands;
using Augur.Application.DTOs;
using Augur.Application.Queries;
using Augur.Domain.Enums;

namespace Augur.Api.Controllers;

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

    [HttpGet("{id:guid}/mappings")]
    [ProducesResponseType(typeof(IReadOnlyList<ControlMappingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ControlMappingDto>>> GetMappings(Guid id, [FromQuery] string? targetFramework)
    {
        var control = await _mediator.Send(new GetControlByIdQuery(id));
        if (control is null) return NotFound();

        if (Enum.TryParse<ComplianceFramework>(targetFramework, ignoreCase: true, out var framework))
        {
            var result = await _mediator.Send(new GetControlMappingsByTargetFrameworkQuery(id, framework));
            return Ok(result);
        }

        var allMappings = await _mediator.Send(new GetControlMappingsQuery(id));
        return Ok(allMappings);
    }

    [HttpGet("mappings/all")]
    [ProducesResponseType(typeof(IReadOnlyList<ControlMappingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ControlMappingDto>>> GetAllMappings()
    {
        var result = await _mediator.Send(new GetAllControlMappingsQuery());
        return Ok(result);
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

    [HttpPost("{sourceControlId:guid}/mappings")]
            [ProducesResponseType(typeof(ControlMappingDto), StatusCodes.Status201Created)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<ActionResult<ControlMappingDto>> CreateMapping(Guid sourceControlId, [FromBody] CreateControlMappingRequest request)
            {
                if (sourceControlId != request.SourceControlId)
                    return BadRequest("SourceControlId in URL must match request body.");

                var sourceControl = await _mediator.Send(new GetControlByIdQuery(sourceControlId));
                if (sourceControl is null) return NotFound($"Source control {sourceControlId} not found.");

                if (!Enum.TryParse<ComplianceFramework>(sourceControl.Framework, ignoreCase: true, out var sourceControlFramework) || request.SourceFramework != sourceControlFramework)
                    return BadRequest("SourceFramework in request must match the source control's framework.");

                var command = new CreateControlMappingCommand(
                    request.SourceControlId,
                    request.SourceFramework,
                    request.SourceControlCode,
                    request.TargetFramework,
                    request.TargetControlCode,
                    request.TargetControlTitle,
                    request.ConfidenceScore,
                    request.Rationale);

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetMappings), new { id = sourceControlId }, result);
            }

        [HttpPut("mappings/{mappingId:guid}")]
        [ProducesResponseType(typeof(ControlMappingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ControlMappingDto>> UpdateMapping(Guid mappingId, [FromBody] UpdateControlMappingRequest request)
        {
            var command = new UpdateControlMappingCommand(mappingId, request.ConfidenceScore, request.Rationale);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("mappings/{mappingId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMapping(Guid mappingId)
        {
            var command = new DeleteControlMappingCommand(mappingId);
            await _mediator.Send(command);
            return NoContent();
        }

        private static ComplianceFramework? ParseFramework(string? framework) => framework?.Trim().ToLowerInvariant() switch
        {
            "soc2" => ComplianceFramework.Soc2,
            "iso27001" => ComplianceFramework.Iso27001,
            null or "" => null,
            _ => throw new ArgumentOutOfRangeException(nameof(framework), "Must be 'Soc2' or 'Iso27001'.")
        };
    }
