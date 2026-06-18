# SuperLeague ⚽

This is my project for managing a football league (inspired by the Serbian SuperLiga).
It's a REST API written in **C# / .NET 8** where you can manage teams and players, and
also pull real data from an external football API. I built it to practice Clean
Architecture, the repository pattern and writing a proper backend, so the code is split
into separate projects instead of one big folder.

> Heads up: there is no authentication yet, so for now the API just uses a hardcoded
> user id (`1`) when something needs a "who did this". That's on my TODO list.

## What it can do

- CRUD for **Teams** (create, read, update, soft-delete + restore)
- CRUD for **Players**
- Linking players to teams (`PlayerTeam`)
- **Soft delete** instead of really deleting rows (so nothing is lost)
- **Optimistic concurrency** using a row version, so two people can't overwrite each
  other by accident
- **Row locking** (a record can be locked by one user, auto-unlocks after 15 minutes)
- Syncing teams + squads from an external football API (api-sports.io)
- Swagger UI so you can click around the endpoints
- A small static front-end (HTML/CSS) in `wwwroot/` (public pages + an admin section)

## Tech stack

- .NET 8 (ASP.NET Core Web API)
- **Dapper** + **Microsoft.Data.SqlClient** for the database (SQL Server)
- **FluentValidation** for validating the incoming DTOs
- **Swashbuckle / Swagger** for API docs
- **NUnit** for tests
- Plain HTML / CSS for the front-end

## How the project is organized

I tried to follow Clean Architecture, so each layer is its own project:

```
SuperLeague/                  -> the Web API (controllers, Program.cs, wwwroot front-end)
SuperLeague.Application/      -> services, DTOs, validators, interfaces, external API stuff
SuperLeague.Domain/           -> the entities (Team, Player, ...) and repository interfaces
SuperLeague.Infrastructure/   -> the actual repositories (Dapper + SQL), external API service
SuperLeague.Tests/            -> NUnit tests
```

The idea is that `Domain` doesn't depend on anything, `Application` only knows about
`Domain`, and `Infrastructure` + the API are the outer layers that plug everything in.

## Getting it to run

**You need:**
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (I used a local SQL Server instance) with the SuperLeague database/tables
- An API key from [api-sports.io](https://www.api-football.com/) if you want the sync to
  work (the free tier is enough)

**Secrets (important):**
The connection string and the football API key are **not** in `appsettings.json` on
purpose (I don't want to commit secrets). I keep them in
[.NET User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets). To set
them up:

```bash
cd SuperLeague
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=SuperLeague;Trusted_Connection=True;TrustServerCertificate=True;"
dotnet user-secrets set "FootballApi:Key" "YOUR_API_KEY_HERE"
```

(You can also just put the key into `appsettings.json` if you don't care about committing
it, but I left it empty.)

**Then run it:**

```bash
dotnet restore
dotnet run --project SuperLeague
```

Open the Swagger page (the URL is printed in the console, something like
`https://localhost:xxxx/swagger`) and you can try the endpoints.

## A few of the endpoints

| Method | Route | What it does |
|--------|-------|--------------|
| GET    | `/api/team`            | list all active teams |
| GET    | `/api/team/{id}`       | get one team |
| POST   | `/api/team`            | create a team |
| PUT    | `/api/team/{id}`       | update a team |
| DELETE | `/api/team/{id}`       | soft-delete a team |
| GET    | `/api/player`          | list players |
| POST   | `/api/datasync/...`    | sync teams/players from the external API |

(There are more — check Swagger for the full list.)

## Tests

There are NUnit tests in `SuperLeague.Tests`. Honest note: they are more like
**integration tests** than unit tests, because they actually talk to a real SQL Server
database (through User Secrets). So you need the database set up for them to pass.

```bash
dotnet test
```

## Things I still want to do (TODO)

- [ ] Add real authentication + users (right now the "created by" user is hardcoded to `1`)
- [ ] Make the tests real unit tests so they don't need a live database
- [ ] Add a global error handler instead of try/catch in every controller
- [ ] Clean up the duplicated cache file

---

This is a learning project, so if you spot something that could be done better I'd love to
know. 🙂
