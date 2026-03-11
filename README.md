# Task Management API

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat-square&logo=csharp)
![EF Core](https://img.shields.io/badge/EF%20Core-8.0-512BD4?style=flat-square)
![MediatR](https://img.shields.io/badge/MediatR-12.3-blue?style=flat-square)
![Docker](https://img.shields.io/badge/Docker-ready-2496ED?style=flat-square&logo=docker)
![CI](https://img.shields.io/badge/CI-GitHub%20Actions-2088FF?style=flat-square&logo=githubactions)
![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)

A **production-ready REST API** for task management, demonstrating Senior-level .NET architecture decisions. Built with **ASP.NET Core 8**, **Clean Architecture**, **CQRS**, and **Domain-Driven Design** principles.

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                        API Layer                         │
│   Controllers · Middleware · Swagger · JWT · DI Setup   │
└────────────────────────┬────────────────────────────────┘
                         │  depends on
┌────────────────────────▼────────────────────────────────┐
│                   Application Layer                      │
│   CQRS (Commands/Queries) · MediatR · Validators        │
│   Pipeline Behaviours (Logging, Validation)             │
└──────────┬────────────────────────────────┬─────────────┘
           │  depends on                    │  depends on
┌──────────▼──────────┐       ┌─────────────▼────────────┐
│    Domain Layer      │       │   Infrastructure Layer   │
│  Entities · Enums   │       │  EF Core · Repositories  │
│  Domain Interfaces  │◄──────│  SQL Server · Auth Svc   │
│  Business Rules     │       │  (implements interfaces) │
└─────────────────────┘       └──────────────────────────┘
```

### Layer Responsibilities

| Layer | Responsibility | Dependencies |
|---|---|---|
| **Domain** | Core business entities and rules | None |
| **Application** | Use-cases, CQRS handlers, validation | Domain |
| **Infrastructure** | Data access, external services | Application, Domain |
| **API** | HTTP endpoints, auth, DI composition | Application, Infrastructure |

---

## Tech Stack

| Category | Technology |
|---|---|
| Framework | ASP.NET Core 8 |
| Language | C# 12 |
| ORM | Entity Framework Core 8 |
| Database | SQL Server 2022 |
| Mediator | MediatR 12 |
| Validation | FluentValidation 11 |
| Authentication | JWT Bearer Tokens |
| API Docs | Swagger / OpenAPI 3 |
| Testing | xUnit + Moq + FluentAssertions |
| Containerisation | Docker + Docker Compose |
| CI/CD | GitHub Actions |

---

## API Endpoints

### Authentication

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/auth/token` | None | Generate a JWT token (demo) |

### Tasks

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `GET` | `/api/v1/tasks` | Bearer | List all tasks (filter by `status`, `assignedTo`) |
| `GET` | `/api/v1/tasks/{id}` | Bearer | Get a task by ID |
| `POST` | `/api/v1/tasks` | Bearer | Create a new task |
| `PUT` | `/api/v1/tasks/{id}` | Bearer | Update an existing task |
| `DELETE` | `/api/v1/tasks/{id}` | Bearer | Soft-delete a task |

### Health

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `GET` | `/health` | None | Application + database health check |

---

## Project Structure

```
dotnet-clean-architecture-api/
├── src/
│   ├── Domain/                          # Core business rules (no dependencies)
│   │   ├── Entities/
│   │   │   └── TaskItem.cs              # Aggregate root with encapsulated behaviour
│   │   └── Interfaces/
│   │       └── ITaskRepository.cs       # Repository contract
│   │
│   ├── Application/                     # Use-cases and CQRS
│   │   ├── Common/
│   │   │   ├── Behaviours/
│   │   │   │   ├── LoggingBehaviour.cs  # MediatR pipeline: structured logging
│   │   │   │   └── ValidationBehaviour.cs # MediatR pipeline: FluentValidation
│   │   │   ├── Exceptions/
│   │   │   │   ├── NotFoundException.cs
│   │   │   │   └── ValidationException.cs
│   │   │   ├── Interfaces/
│   │   │   │   └── ICurrentUserService.cs
│   │   │   └── Models/
│   │   │       ├── TaskDto.cs
│   │   │       └── PaginatedResult.cs
│   │   ├── Tasks/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateTask/          # Command + Handler + Validator
│   │   │   │   ├── UpdateTask/          # Command + Handler
│   │   │   │   └── DeleteTask/          # Command + Handler
│   │   │   └── Queries/
│   │   │       ├── GetAllTasks/         # Query + Handler
│   │   │       └── GetTaskById/         # Query + Handler
│   │   └── DependencyInjection.cs
│   │
│   ├── Infrastructure/                  # Data access and external services
│   │   ├── Authentication/
│   │   │   └── CurrentUserService.cs    # ICurrentUserService via IHttpContextAccessor
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── AppDbContextSeed.cs      # Dev seed data
│   │   │   └── Configurations/
│   │   │       └── TaskItemConfiguration.cs  # Fluent API entity config
│   │   ├── Repositories/
│   │   │   └── TaskRepository.cs        # EF Core repository implementation
│   │   └── DependencyInjection.cs
│   │
│   └── API/                             # Presentation layer
│       ├── Controllers/
│       │   ├── TasksController.cs
│       │   └── AuthController.cs        # Demo JWT generation
│       ├── Extensions/
│       │   ├── SwaggerExtensions.cs
│       │   └── JwtExtensions.cs
│       ├── Middleware/
│       │   └── ExceptionHandlingMiddleware.cs  # Global error handling → RFC 7807
│       ├── Program.cs
│       └── appsettings.json
│
├── tests/
│   └── Application.Tests/
│       └── Tasks/
│           └── CreateTaskHandlerTests.cs  # 8 unit tests (xUnit + Moq + FluentAssertions)
│
├── .github/
│   └── workflows/
│       └── ci.yml                       # Build → Test → Quality → Docker → Publish
│
├── Dockerfile                           # Multi-stage build
├── docker-compose.yml                   # API + SQL Server
└── dotnet-clean-architecture-api.sln
```

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for containerised run)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (if running locally without Docker)

---

### Option 1 — Run with Docker (recommended)

```bash
# Clone the repo
git clone https://github.com/your-username/dotnet-clean-architecture-api.git
cd dotnet-clean-architecture-api

# Start everything (SQL Server + API)
docker compose up --build

# API will be available at:
#   Swagger UI  → http://localhost:5000
#   Health check → http://localhost:5000/health
```

---

### Option 2 — Run locally

```bash
# 1. Ensure SQL Server is running and update the connection string in:
#    src/API/appsettings.Development.json

# 2. Restore, build, and apply EF migrations
cd src/API
dotnet restore
dotnet ef database update --project ../Infrastructure/Infrastructure.csproj

# 3. Run the API
dotnet run

# Swagger UI → https://localhost:5001
```

---

### Running Tests

```bash
dotnet test tests/Application.Tests/Application.Tests.csproj \
  --logger "console;verbosity=detailed" \
  --collect:"XPlat Code Coverage"
```

---

## Authentication Flow

This project ships with a lightweight JWT demo endpoint. In a real application, replace `AuthController` with ASP.NET Core Identity, Duende IdentityServer, or Azure AD B2C.

```bash
# 1. Generate a token
curl -s -X POST http://localhost:5000/api/v1/auth/token \
  -H "Content-Type: application/json" \
  -d '{"username": "demo"}' | jq .

# Response:
# {
#   "accessToken": "eyJhbGci...",
#   "expiresAt": "2024-...",
#   "tokenType": "Bearer"
# }

# 2. Use the token in subsequent requests
curl -s http://localhost:5000/api/v1/tasks \
  -H "Authorization: Bearer eyJhbGci..."
```

---

## Create Task — Example Request

```bash
curl -s -X POST http://localhost:5000/api/v1/tasks \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Implement OAuth2 login",
    "description": "Add Google and GitHub social login providers.",
    "priority": 2,
    "dueDate": "2024-12-31T00:00:00Z",
    "assignedTo": "dev@company.com"
  }'
```

Priority values: `0` = Low, `1` = Medium, `2` = High, `3` = Critical

Status values: `0` = Todo, `1` = InProgress, `2` = Done, `3` = Cancelled

---

## Design Decisions

### Why Clean Architecture?
Dependencies flow inward — the Domain and Application layers have zero knowledge of ASP.NET Core or SQL Server. This allows the core business logic to be tested in complete isolation and infrastructure to be swapped without touching the domain.

### Why CQRS with MediatR?
Commands mutate state; queries read state. Keeping them separate makes the intent of each use-case explicit, reduces handler complexity, and opens the door to separate read/write database optimisations (e.g., read replicas, Dapper for queries).

### Why MediatR Pipeline Behaviours?
Cross-cutting concerns (logging, validation, caching) are added as pipeline decorators and applied uniformly to all handlers without any handler being aware of them. This is the open/closed principle in action.

### Why Soft Deletes?
Data deleted by users is flagged `IsDeleted = true` rather than removed from the database. This supports audit trails, recovery scenarios, and referential integrity. A global EF Core query filter ensures deleted records are never returned.

### Why encapsulate entity state?
`TaskItem` exposes only `private set` properties and factory/mutation methods (`Create`, `UpdateDetails`, `UpdateStatus`, `SoftDelete`). This enforces business invariants at the domain level rather than relying on callers to do the right thing.

---

## CI/CD Pipeline

```
push / PR
    │
    ▼
[build-and-test]  — dotnet restore → build → xUnit tests → coverage
    │
    ├──► [code-quality]   — dotnet format --verify-no-changes
    ├──► [docker-build]   — Docker multi-stage build (no push)
    └──► [security-scan]  — dotnet list package --vulnerable
              │
         (main only)
              ▼
         [publish]  — push image to ghcr.io with SHA + latest tags
```

---

## Environment Variables

| Variable | Default | Description |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | (see appsettings) | SQL Server connection string |
| `JwtSettings__SecretKey` | (see appsettings) | HMAC-SHA256 signing key — change in production |
| `JwtSettings__Issuer` | `TaskManagementAPI` | JWT issuer claim |
| `JwtSettings__Audience` | `TaskManagementClients` | JWT audience claim |
| `JwtSettings__ExpiryMinutes` | `60` | Token lifetime in minutes |
| `ASPNETCORE_ENVIRONMENT` | `Production` | Enables/disables Swagger, seed data |

---

## License

This project is licensed under the [MIT License](LICENSE).
