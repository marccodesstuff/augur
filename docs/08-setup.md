# Setup

## Prerequisites

- .NET 10 SDK
- PostgreSQL 16+ (or Docker)
- Node.js + `npm` (scss compile only)
- LM Studio optional (agent works offline)

## Database

```bash
# Docker
docker run -d --name minigrc-pg \
  -e POSTGRES_PASSWORD=1234 \
  -e POSTGRES_DB=minigrc \
  -p 5432:5432 postgres:16
```

Update `src/MiniGrc.Api/appsettings.json` connection string to match your creds. The fallback in `Program.cs` must be updated too.

## Run

```bash
# Terminal 1: API (migrations run automatically in Development)
dotnet run --project src/MiniGrc.Api/MiniGrc.Api.csproj --urls http://localhost:5050

# Terminal 2: Web
dotnet run --project src/MiniGrc.Web/MiniGrc.Web.csproj --urls http://localhost:5000
```

## Compile component styles

```bash
cd src/MiniGrc.ComponentLib
npm install
npx sass
```

## Troubleshooting

- `28P01: password authentication failed`: password in `appsettings.json` / `Program.cs` fallback doesn’t match the Postgres `postgres` user password. Update both.
- `MSB3021` / `MSB3027` during build: a running `dotnet run` process holds DLL locks. Kill `dotnet.exe` and rebuild.
- `/mcp` returns 405: must be `POST`. `tools/list` and `tools/call` only.
- `/mcp` returns 404: route mounted after `MapControllers()` in `Program.cs`. Ensure `app.MapPost("/mcp", ...)` is registered.
