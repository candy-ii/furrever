# Furrever

An Angular frontend talking to an ASP.NET Core Web API, which stores its data in Postgres. PostgreSQL runs as a system service.

    Backend/     ASP.NET Core Web API   -> Postgres
    Frontend/    Angular app            -> Backend

## Where things live

"Backend/Models/" is where the models are. "Backend/Controllers/" is where the controllers are. (The '[Route("api/[controller]")]' line on it is what turns the class name 'Pets' into the address "/api/pets".)

The View is from the Angular side, not the backend; ASP.NET here renders no HTML at all. "Frontend/src/app/app.html" is the page actually seen, and "app.ts" beside it is its code. Anything that talks to the backend belongs in "pet-api.ts", not in a component. New pages are registered in "app.routes.ts" and appear inside the "<router-outlet />" at the bottom of "app.html".

"Backend/Data/AppDbContext.cs" is the database connection itself, and every "DbSet" line on it is one table.

The database lives at "/var/lib/postgresql/17"; the project only knows how to reach it, via "Backend/.env".

"UseAuthorization()" from 'dotnet new webapi' is not there because there's nothing to authorize yet. It needs to be put back when logins exist.

## The database

`furrever_dev` is created on first run (so ".env" needs a login with createdb rights) and seeded with two sample rows. Inspect it with `psql furrever_dev`; `dropdb furrever_dev` wipes it and the next `dotnet run` rebuilds it.

### Changing the schema (migrations)

Editing a model does not change the database by itself. Adding a property to "Pet.cs" needs a migration — a generated C# file holding the SQL for that one change:

    dotnet ef migrations add AddBreed     # writes the migration
    dotnet ef database update             # applies it, existing rows intact

Migrations live in "Backend/Migrations" and are committed like any other code: they are not a changelog, they are the only instructions that know how to build the tables. Without them there is no schema.

They stack up as a series because each database is at a different point. On startup "Program.cs" calls `Migrate()`, which checks the "__EFMigrationsHistory" table and runs only what's missing — so a missing database gets built from scratch, an out-of-date one catches up without losing rows, and an up-to-date one is left alone.

`dotnet ef` is pinned as a local tool in "Backend/dotnet-tools.json" — run `dotnet tool restore` once after cloning. Every `dotnet ef` command runs from inside "Backend/".

## How the three are connected

**Angular to the backend.**

"Frontend/proxy.conf.json" tells the Angular dev server to forward anything starting with '/api' to the backend on port 5037, and it's registered in 'angular.json' under the serve target.

The other necessary piece is `provideHttpClient()` in "app.config.ts", without which Angular services can't make HTTP calls.

**The backend to Postgres.**

Runs on the "Npgsql.EntityFrameworkCore.PostgreSQL" package over TCP. Host, port, database, username and password come from "Backend/.env", read by "Program.cs" at startup. "appsettings.json" holds no connection details, and ".env" is gitignored — copy ".env.example" to ".env" and fill it in. It's the only file that differs between machines.

**Machine setup.** Needs the .NET SDK, Node and Postgres installed. Postgres needs a login before anything can connect, so copy "Backend/.env.example" to "Backend/.env" and fill in its credentials. Then, in two terminals:

    cd Backend  && dotnet run                 # http://localhost:5037
    cd Frontend && npm install && npm start   # http://localhost:4200

"node_modules" is gitignored, so `npm install` is required on a fresh clone; after that `npm start` alone is enough. Go to http://localhost:4200. Both halves need to be running: the frontend has no data of its own, so if the backend is down the page just says "No pets yet".
