# Hybrid Library System

A hybrid library management system built with **ASP.NET Core 10** (MVC + Identity + Web API) and a **React + Vite** single-page application. Books, authors, and categories are managed through the web app, and the SPA is served under `/app` by the backend.

## Project structure

| Folder                          | Description                                                                                   |
| ------------------------------- | --------------------------------------------------------------------------------------------- |
| `HybridLibrarySystem.Core`      | Domain models, DTOs, repository interfaces, and authorization definitions (roles/permissions) |
| `HybridLibrarySystem.Data`      | EF Core `LibraryContext` (SQL Server) and database migrations                                 |
| `HybridLibrarySystem.Web`       | ASP.NET Core host: MVC controllers, Razor Identity UI, Web API, and seeding                   |
| `HybridLibrarySystem.ClientApp` | React + Vite + Tailwind SPA (pnpm). Builds into `HybridLibrarySystem.Web/wwwroot/app`         |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) and [pnpm](https://pnpm.io/) (`corepack enable` is an easy way to get the pinned pnpm version)
- SQL Server. The default connection string targets **SQL Server LocalDB**, which ships with Visual Studio. Any SQL Server / SQL Express instance works too.
- For the `dotnet` migration workflow: the EF Core CLI tool: `dotnet tool install --global dotnet-ef`

## 1. Set the connection string

The app reads the connection string named `LibraryContext`. Default value in `HybridLibrarySystem.Web/appsettings.json`:

```json
"ConnectionStrings": {
  "LibraryContext": "Server=(localdb)\\mssqllocaldb;Database=LibraryContext;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

Pick **one** of these ways to set it:

- **Edit `appsettings.json`** directly (fine for local dev, don't commit real credentials).
- **User secrets** (recommended for development): run from the `HybridLibrarySystem.Web` folder:

  ```sh
  dotnet user-secrets set "ConnectionStrings:LibraryContext" "Server=localhost;Database=LibrarySystem;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  ```

- **Environment variable** (recommended for deployment):

  ```sh
  setx ConnectionStrings__LibraryContext "Server=...;Database=...;..."
  ```

## 2. Create / update the database (apply migrations)

Migrations live in `HybridLibrarySystem.Data/Migrations`. There are **two ways** to apply them to your database:

### Option A - run the included `efbundle.exe` (no SDK needed)

A pre-built [EF Core migration bundle](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying#bundles) is included in the repo root. It applies all migrations in one shot and requires no .NET tooling at the machine where you run it. Give it the connection string with `--connection` (it does not read your user secrets):

```sh
.\efbundle.exe --connection "Server=(localdb)\mssqllocaldb;Database=LibraryContext;Trusted_Connection=True;MultipleActiveResultSets=true"
```

- It's safe to re-run, already-applied migrations are skipped (`No migrations were applied. The database is already up to date.`).
- To roll back, pass a target migration name (or `0` to revert everything), e.g. `.\efbundle.exe 0 --connection "..."`. Rollbacks can lose data, use with care.

### Option B - the `dotnet ef` CLI

```sh
dotnet ef database update --project HybridLibrarySystem.Data --startup-project HybridLibrarySystem.Web
```

The startup project is `HybridLibrarySystem.Web` because that's where the connection string and `UseSqlServer` are configured. With the default LocalDB connection string, this creates the `LibraryContext` database automatically.

> **Tip:** If you already applied Option A or B, you do **not** need to run the other, they use the same `__EFMigrationsHistory` table.

## 3. Build the client app (SPA)

Building the C# projects (`dotnet build` / `dotnet publish`) does **not** build the React app. To make `HybridLibrarySystem.Web` serve your current client code at `/app`, build `HybridLibrarySystem.ClientApp` first:

```sh
cd HybridLibrarySystem.ClientApp
pnpm install      # first time, or when package.json / pnpm-lock.yaml has changed
pnpm run build
```

This runs TypeScript checks + the Vite production build and puts the output in `HybridLibrarySystem.Web/wwwroot/app` (see `outDir` in `vite.config.ts`). Restart the backend afterwards so freshly built assets are picked up.

Notes:

- In **Visual Studio** you don't need to do this manually, the solution includes the client project (`HybridLibrarySystem.ClientApp.esproj`), and building the solution runs `pnpm install` (when `node_modules` is missing) followed by `pnpm build` automatically.
- Rebuild whenever you want the latest client changes visible at `https://localhost:7238/app`. For active development you don't need to rebuild at all, use the Vite dev server (`pnpm run dev`) with hot reload as described in the next section.

## 4. Run the app

Run the backend (it's required for both the SPA and the Identity pages):

```sh
dotnet run --project HybridLibrarySystem.Web --launch-profile https
```

This starts the server at **https://localhost:7238** (an HTTP copy listens on `http://localhost:5168`).

Then choose one of:

- **Full SPA dev experience (recommended):** in a second terminal, from `HybridLibrarySystem.ClientApp`:

  ```sh
  pnpm install
  pnpm run dev
  ```

  Open **http://localhost:5173**. The Vite dev server proxies `/api` and `/Identity` requests to `https://localhost:7238`, so cookies/auth work same-origin.

- **Without the dev server:** open **https://localhost:7238/app**, the production build of the SPA (from step 3) is served as static assets from `wwwroot/app`.
- **Classic MVC pages:** the backend's own Razor pages/controllers are directly available at **https://localhost:7238** (e.g. the Identity login at `/Identity/Account/Login`).

API docs (OpenAPI + Scalar) are available in development at **https://localhost:7238/api/docs**.

## 5. Making a new migration

When you change the model (in `HybridLibrarySystem.Core` / `HybridLibrarySystem.Data`), create a migration:

```sh
dotnet ef migrations add <MigrationName> --project HybridLibrarySystem.Data --startup-project HybridLibrarySystem.Web
```

Then apply it using **either** method from step 2.

If you change the model, the bundled `efbundle.exe` becomes stale. Regenerate it with:

```sh
dotnet ef migrations bundle --project HybridLibrarySystem.Data --startup-project HybridLibrarySystem.Web --force
```

(`--force` overwrites the existing `efbundle.exe`.)

## 6. Default admin account

On the first successful startup, the app seeds roles and a default admin user **only if the users table is empty**:

|              |                     |
| ------------ | ------------------- |
| **Username** | `admin`             |
| **Email**    | `admin@example.com` |
| **Password** | `Admin123!`         |
| **Role**     | `Admin`             |

- Log in at `/Identity/Account/Login` (the SPA can also route you there).
- Three roles are seeded with permission claims: **Admin** (all permissions), **Librarian**, and **Member**.
- If a user already exists in the database, the default admin is **not** re-created. To redo the seeding, drop the `AspNetUsers` rows (or the whole database) and restart the app.
- **Change the default password immediately**, it is well known.

## Troubleshooting

- **`Connection string 'LibraryContext' not found`** - the effective configuration didn't contain it; check the section above.
- **SPA shows stale content** - the backend is serving the last client build. Rebuild `HybridLibrarySystem.ClientApp` (step 3), then restart the backend.
- **Vite proxy errors** - make sure the backend is actually running on `https://localhost:7238` (the `.env.production`/proxy assumes that port). Change the `backend` constant in `HybridLibrarySystem.ClientApp/vite.config.ts` if you use a different port.
