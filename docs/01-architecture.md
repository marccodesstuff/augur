# Architecture

Mini-GRC follows strict Onion Architecture. Dependencies point inward only.
The Domain owns the business rules; outer layers are plugins.

```
┌──────────────────────────────────────────────────────────────────────┐
│  MiniGrc.Web (Blazor)  ·  MiniGrc.ComponentLib                      │  Presentation
└───────────────┬──────────────────────────────────────────────────────┘
                │ HttpClient (typed ApiClient)
┌───────────────▼──────────────────────────────────────────────────────┐
│  MiniGrc.Api (ASP.NET Core, OpenAPI 3.1.1, /mcp bridge)              │  API / Host
└───────────────┬──────────────────────────────────────────────────────┘
                │ MediatR
┌───────────────▼──────────────────────────────────────────────────────┐
│  MiniGrc.Application (CQRS, validators, pipeline, mappings)          │  Application
└───────┬───────────────────────────────┬──────────────────────────────┘
        │ ports (IUnitOfWork, I*Repository)│
┌───────▼───────────────┐  ┌──────────────▼───────────────────────────┐
│ MiniGrc.Domain        │  │ MiniGrc.Infrastructure (EF + Npgsql)      │
│ entities, enums,      │  │ DbContext, repo impls, migrations         │
│ repository ports      │  └──────────────────────┬──────────────────┘
└───────────────────────┘                         │
                                         ┌─────────▼─────────┐
                                         │  MiniGrc.Agent     │  (uses Application + Domain)
                                         │ LLM client +       │
                                         │ deterministic brain│
                                         └────────────────────┘
```

## Layers

- **Domain**: entities, enums, repository interfaces, `IUnitOfWork`. No framework dependencies beyond `System` and `MediatR` primitives.
- **Application**: commands/queries, MediatR handlers, FluentValidation, pipeline behaviors, Mapster mappings. Depends on Domain interfaces only.
- **Infrastructure**: EF Core `DbContext`, repository implementations, PostgreSQL migrations. Implements Domain ports; never referenced by inner layers.
- **Api**: ASP.NET Core controllers, OpenAPI generation, CORS, `/mcp` JSON-RPC bridge. Wires Application + Infrastructure + Agent.
- **Agent**: LLM client (`OpenAiCompatibleClient`), `ControlCatalog`, `DeterministicAnalyzer`. Depends on Application and Domain; registered as scoped services.
- **Web**: Blazor Server front end (Dashboard, Controls, Evidence, Agent pages). `ComponentLib` reusable UI with scoped `.scss` + `data-testid` selectors.
- **ComponentLib**: shared Blazor components (GrcBase pattern), compiled scoped styles, used by Web.

## Dependency rule

No outer layer may reference an inner layer. The only assemblies that reference EF Core, Npgsql, or ASP.NET Core are `Infrastructure` and `Api`.
