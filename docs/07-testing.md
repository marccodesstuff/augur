# Testing

Two test projects, no shared bootstrap magic.

## Unit tests (`MiniGrc.UnitTests`)

Run against in-memory `IUnitOfWork` implementations. Controllers and handlers are exercised without EF Core or Postgres.

```bash
dotnet test tests/MiniGrc.UnitTests/MiniGrc.UnitTests.csproj
```

Current: 4 handler tests passing.

## E2E tests (`MiniGrc.E2E`)

Playwright drives the running API + Web apps. Selectors are **`data-testid` only** — no CSS selectors.

Flows covered:
1. Create a control
2. Upload evidence
3. Run the agent
4. View compliance status

```bash
# Apps must be running on their configured ports
dotnet test tests/MiniGrc.E2E/MiniGrc.E2E.csproj
```

Current: 5 Playwright tests passing.

## Design rules

- Unit tests never start the API host.
- E2E tests never use CSS selectors, hard waits, or test IDs buried in markup that doesn’t represent a user action.
