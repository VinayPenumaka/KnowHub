# KnowHub

KnowHub is a deliberately small, extendable knowledge-management application. V1 manages projects, notes, tasks, documents, keyword search, and activity. It does **not** make AI calls, extract document text, generate embeddings, or use vector search.

## Architecture

`React page → service module → Axios → ASP.NET controller → application service → EF Core DbContext → PostgreSQL`

The browser never talks to PostgreSQL. A controller translates HTTP into an application call; the service holds the use-case logic; `AppDbContext` maps C# objects to relational data. The response returns through the same path as JSON for React to render.

DTOs are the boundary data shapes used by the API. They stop the database entities (which include relationships and internal fields) being exposed or accepted directly. Dependency injection is configured in `Program.cs`: the framework creates a controller and supplies the service interface it requests. This keeps each class focused and easy to replace or test.

## Layout

- `KnowHub.Api/Models` — EF Core database entities and task status enum.
- `Data` — DbContext, relationship configuration, and development seeding.
- `DTOs` — validated API input and safe API output shapes.
- `Services` — application rules; `IFileStorage` lets local disk storage be swapped for S3/Azure later.
- `Controllers` — REST endpoints only, with minimal HTTP translation.
- `Middleware` — catches unhandled errors and returns `{ success, data, message }`.
- `Services/AI` — an intentionally empty extension point for a later provider.
- `knowhub-client/src/services` — one Axios instance and endpoint-focused service modules.
- `knowhub-client/src/pages` — dashboard, projects, login placeholder, and project workspace.

## Start it
dotnet ef migrations add InitialCreate --project KnowHub.Api
Prerequisites: .NET 8 SDK, Node.js 20+, Docker Desktop (optional), and PostgreSQL 16 (or Docker).

1. Copy `.env.example` to `.env` and set `ConnectionStrings__DefaultConnection` if your local PostgreSQL credentials differ. The API reads that environment variable first; never commit `.env`.
2. Start PostgreSQL with `docker compose up -d postgres` (the included development credentials are `postgres` / `postgres`) or use your own local instance.
3. From the repository root: `dotnet restore`, then install EF tooling if needed: `dotnet tool install --global dotnet-ef`.
4. Create the first migration and database: `` then `dotnet ef database update --project KnowHub.Api`. Migrations are the versioned record of database changes; do not replace them with raw SQL.
5. Start the API: `dotnet run --project KnowHub.Api --urls http://localhost:5000`. Seed data appears on first startup.
6. In a second terminal: `cd knowhub-client`, `npm install`, copy `.env.example` to `.env`, then `npm run dev`.

Open the frontend URL Vite prints (normally `http://localhost:5173`) and Swagger at `http://localhost:5000/swagger`.

## API map

| Area | Endpoints |
| --- | --- |
| Projects | `GET/POST /api/projects`, `GET/PUT/DELETE /api/projects/{id}` |
| Notes | `GET/POST /api/projects/{projectId}/notes`, `GET/PUT/DELETE /api/notes/{id}`, `GET /api/notes/search?q=api` |
| Tasks | `GET/POST /api/projects/{projectId}/tasks`, `PUT/DELETE /api/tasks/{id}` |
| Documents | `GET/POST /api/projects/{projectId}/documents`, `GET/DELETE /api/documents/{id}` |
| Activity | `GET /api/projects/{projectId}/activities` |

Example project body: `{ "name": "Python Learning", "description": "Learn Python APIs" }`. Note body: `{ "title": "What is HTTP?", "content": "..." }`. Task body: `{ "title": "Build first API", "status": "Pending", "dueDate": null }`. Document upload uses `multipart/form-data` with a `file` field.

HTTP status conventions: 200 means a successful read/update; 201 a newly created resource; 204 a completed delete; 400 malformed input; 404 missing resources; 500 an unexpected server error. ASP.NET automatic model validation returns 400 with field errors, and the exception middleware gives all other errors a consistent JSON envelope.

## What comes next

Authentication/JWT is intentionally the next discrete learning task. Later, a Python AI service can be introduced behind `IAIService` or an HTTP client without changing the React-to-API or application-data flow. PostgreSQL can then add pgvector alongside normal tables for embeddings, keyword-plus-semantic search, and RAG.
