# Deployment

## Environment variables

- `ConnectionStrings__MiniGrc` — required in non-Development environments.
- `Agent__LlmEndpoint` — optional LLM endpoint; empty string = deterministic-only.
- `Agent__Model` — model id passed to the LLM endpoint.

## Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
COPY bin/ .
ENTRYPOINT ["dotnet", "MiniGrc.Api.dll"]
```

Build:

```bash
dotnet publish src/MiniGrc.Api/MiniGrc.Api.csproj -c Release -o ./publish
docker build -t minigrc-api -f src/MiniGrc.Api/Dockerfile ./publish
```

Run:

```bash
docker run -p 5050:8080 \
  -e ConnectionStrings__MiniGrc="Host=pg;Port=5432;Database=minigrc;Username=postgres;Password=..." \
  minigrc-api
```

## Notes

- `MiniGrc.Web` is a separate deployment. In production, pre-compile scss and serve the static Blazor bundle from the same origin or configure CORS explicitly.
- `db.Database.Migrate()` should be gated behind a release migration step, not startup, in production.
