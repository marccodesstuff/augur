# Database

EF Core + PostgreSQL via Npgsql. Single DbContext in Infrastructure.

## Model

- **Control** aggregate: `Control` + child `Evidence` (cascade delete).
- **Finding** aggregate: `Finding` + child `RemediationTask` (cascade delete).
- **Risk**: standalone aggregate.

## Key EF decisions

- Enum properties (`Framework`, `Status`, `Severity`, `Priority`) stored as `int` via `HasConversion<int>()`.
- `Control.Code` has a unique index.
- `Finding.ExternalId` has a non-unique index for dedup lookups.
- `Evidence` and `RemediationTask` navigation properties use field access (`PropertyAccessMode.Field`).
- `Risk.Severity` is ignored by EF (computed from other fields in the domain).

## Migrations

Run automatically on startup in Development via `db.Database.Migrate()`.
Generated with `dotnet ef migrations add <name>` from the `Infrastructure` project.

## Connection string

`appsettings.json`:

```json
{ "ConnectionStrings": { "MiniGrc": "Host=localhost;Port=5432;Database=minigrc;Username=...;Password=..." } }
```

Fallback in `Program.cs` if config is missing. Change it once, both `Main` and `CreateApp` use it.

## Why this shape

No repository-per-entity interfaces. One `IUnitOfWork` port keeps the Application layer decoupled from EF Core while avoiding an explosion of narrow interfaces. The DbContext never leaks outside Infrastructure.
