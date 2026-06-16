SQLite setup and optional migrations

This project is configured to use SQLite by default for demo/presentation purposes.

What I changed
- `appsettings.json` now uses `Data Source=estufa.db` as the `DefaultConnection`.
- `Program.cs` will attempt to apply EF migrations if present, otherwise it will call `Database.EnsureCreated()` and then run the DB seeder.

Running locally (no .NET SDK installed?)
- If you have the .NET SDK, run:

```bash
cd backend/Estufa.Api
dotnet restore
dotnet run
```

- The API will create `estufa.db` next to the application and seed demo data.

Creating EF Core migrations (optional)
- If you want real EF migrations (recommended for production), and you have the .NET SDK installed:

```bash
cd backend/Estufa.Api
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

- After creating migrations, they will be applied automatically on startup via `Database.Migrate()`.

Notes
- `EnsureCreated()` is used as a safe fallback for demo environments without migration files; it will create schema directly and is suitable for lightweight demos.
- If you later generate migrations, delete the database file (`estufa.db`) and run `dotnet ef database update` to regenerate from migrations.

Quick run & test (Windows PowerShell)

1) Execute the helper script (repositório root):
```powershell
.\scripts\run_and_test_api.ps1
```

2) Manual commands if you prefer:
```powershell
cd backend/Estufa.Api
dotnet restore
dotnet run
```

3) Endpoints (endereços esperados):
- API base (HTTP): http://localhost:5000
- Swagger UI (se em Development): https://localhost:5001/swagger

Exemplos de teste (PowerShell):
```powershell
# current weather
Invoke-WebRequest -Uri 'http://localhost:5000/api/meteorologia/atual' -UseBasicParsing
# recent readings
Invoke-WebRequest -Uri 'http://localhost:5000/api/sensores/leituras?minutes=60' -UseBasicParsing
# reservoir
Invoke-WebRequest -Uri 'http://localhost:5000/api/reservatorios/principal' -UseBasicParsing
```
