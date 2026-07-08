using MediatR;
using Microsoft.AspNetCore.Mvc;
using MiniGrc.Agent;
using MiniGrc.Agent.Models;
using MiniGrc.Api.Requests;
using MiniGrc.Domain.Enums;

namespace MiniGrc.Api.Controllers;

[ApiController]
[Route("api/v1/agent")]
[Produces("application/json")]
public sealed class AgentController : ControllerBase
{
    private readonly ComplianceAgentService _agent;

    public AgentController(ComplianceAgentService agent) => _agent = agent;

    [HttpPost("run")]
    [ProducesResponseType(typeof(AgentResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<AgentResult>> Run([FromBody] RunAgentRequest request)
    {
        if (!Enum.TryParse<ComplianceFramework>(request.Framework, ignoreCase: true, out var framework))
            return BadRequest("Framework must be 'Soc2' or 'Iso27001'.");

        var agentRequest = new AgentRequest(request.Source, request.Format, request.Content, framework);
        var result = await _agent.RunAndPersistAsync(agentRequest);
        return Ok(result);
    }
}
