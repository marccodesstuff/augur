# Agent design

`MiniGrc.Agent` turns raw security input into mapped, actionable findings. This is Mini-GRC’s answer to “building secure, autonomous software agents” and “AI orchestration.”

## Components

- `ComplianceAgentService` — public entry point; persists findings through `IUnitOfWork`.
- `ComplianceAgent` — core orchestration; accepts a `ComplianceAgent` strategy (LLM or deterministic).
- `OpenAiCompatibleClient` — minimal OpenAI-compatible HTTP client (`/v1/chat/completions`). Works with LM Studio, Ollama, OpenAI.
- `DeterministicAnalyzer` — offline-safe fallback.
- `ControlCatalog` — static control library; keyword-based mapping to SOC 2 / ISO 27001 controls.
- `AgentModels` — `AgentRequest`, `AgentResult`, `AgentFinding`, `AgentRemediation`.

## Flow

1. Caller supplies `source`, `format` (`json` / `text`), `content`, and `framework`.
2. If `Agent:LlmEndpoint` is configured, `ComplianceAgent` sends the framed prompt to the LLM and expects strict JSON.
3. If the LLM is unreachable, times out, or returns malformed JSON, the agent degrades to `DeterministicAnalyzer` without throwing.
4. Findings are mapped to controls via `ControlCatalog.MapToControlCode`.
5. Remediation tasks are drafted with priority scaled to severity.
6. `ComplianceAgentService` persists the results (de-duped by `ExternalId`) via `IUnitOfWork`.

## Failure modes

| Failure | Behavior |
|---------|----------|
| No `Agent:LlmEndpoint` in config | Deterministic-only path; works offline. |
| LLM unreachable / timeout | Catches exception; falls back to deterministic. |
| Malformed LLM response (markdown fences, extra prose) | Strips fences; reparses. Still falls back if parse fails. |
| Empty findings | Returns empty `Findings` list, `MappedCount = 0`, non-null `RiskSummary`. |

## Key design choices

- **The LLM is an enhancement, never a hard dependency.** The product always produces a result.
- **Prompt framing is fixed, not dynamic.** Prompt engineering is kept in code rather than prompt files so failures are versioned and reviewable.
- **No streaming.** Agent runs are bounded and synchronous. Keeps orchestration inspectable in logs and easy to test.
