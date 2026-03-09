# SnippetVault API

A RESTful Web API for managing personal code snippets, built with Clean Architecture principles on .NET 10.

## 🔗 Live
**API Explorer:** https://snippetvault-api.onrender.com/scalar

> The free tier on Render.com spins down after 15 minutes of inactivity. The first request may take 30–60 seconds to wake up — subsequent requests are fast.

## 🔗 Related Repositories
- [SnippetVault-Client](https://github.com/janlu89/SnippetVault-Client) — Angular 21 frontend
- [SnippetVault-Infra](https://github.com/janlu89/SnippetVault-Infra) — Docker Compose orchestration

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Runtime | .NET 10 / C# 14 |
| Framework | ASP.NET Core Web API |
| ORM | Entity Framework Core 10 |
| Database (dev) | SQLite |
| Database (prod) | PostgreSQL 17 (Neon.tech) |
| Auth | JWT Bearer + BCrypt |
| Logging | Serilog (console + rolling file) |
| Testing | xUnit + Moq |
| API Docs | OpenAPI + Scalar UI |
| Containerisation | Docker multi-stage build |

---

## Architecture

Clean Architecture with four layers:

```
SnippetVault.Domain          → Entities only, no dependencies
SnippetVault.Application     → Interfaces, DTOs, Service contracts
SnippetVault.Infrastructure  → EF Core, Repositories, TokenService
SnippetVault.API             → Controllers, Middleware, Program.cs
```

Dependencies flow inward — Domain has no dependencies, Application depends only on Domain, Infrastructure implements Application interfaces, API wires everything together.

---

## Features

- **JWT Authentication** — register and login with BCrypt password hashing
- **Full Snippet CRUD** — create, read, update, soft delete, hard delete
- **Pagination** — page and pageSize parameters on all list endpoints
- **Filtering** — search by title/description, filter by language and tag
- **Soft Delete** — global EF Core query filters, deleted records never surface in queries
- **Tag Management** — free-form tags, normalised to lowercase, created on the fly
- **Ownership Validation** — users can only edit and delete their own snippets
- **Global Exception Middleware** — consistent error responses across all endpoints
- **Environment-aware Configuration** — SQLite locally, PostgreSQL in production

---

## Running Locally

### With Docker Compose (recommended — runs full stack)
See [SnippetVault-Infra](https://github.com/janlu89/SnippetVault-Infra) for the full stack setup.

### Without Docker

**Prerequisites:** .NET 10 SDK

```bash
git clone https://github.com/janlu89/SnippetVault-API.git
cd SnippetVault-API
dotnet run --project SnippetVault.API
```

The API will be available at `https://localhost:7084/scalar`

The app uses SQLite in Development mode — the database file is created automatically on first run via `EnsureCreated()`, no migrations needed.

---

## Running Tests

```bash
dotnet test
```

15 unit tests covering service layer logic and JWT authentication.

---

## Environment Variables

| Variable | Description |
|----------|-------------|
| `ASPNETCORE_ENVIRONMENT` | `Production` or `Development` |
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string (prod) or SQLite path (dev) |
| `JwtSettings__SecretKey` | JWT signing key — minimum 32 characters |
| `JwtSettings__Issuer` | Token issuer |
| `JwtSettings__Audience` | Token audience |
| `JwtSettings__ExpirationInMinutes` | Token lifetime in minutes |
| `AllowedOrigins__0` | First allowed CORS origin |
| `AllowedOrigins__1` | Second allowed CORS origin (optional) |
