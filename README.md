# TutorConnect-Docker

---

# Activity: Tutor Booking Mini‑App (MVC, DB‑First) with SQL Server & Docker

## Learning goals

* Stand up SQL Server via Docker and apply a schema.
* Reverse‑engineer (DB‑First) EF Core models and DbContext.
* Scaffold MVC controllers & views for CRUD.
* Run the whole stack with `docker-compose`.

## You’ll build

A simple CRUD web app for **Students**, **Tutors**, and **Sessions** (student‑tutor bookings).

## Prerequisites

* Windows 10/11, **Docker Desktop**, **Visual Studio 2022** (latest), **.NET 8 SDK**.
* Visual Studio Components: **ASP.NET and web development**, **.NET 8**.
* (Optional) **SQL Server Management Studio** for quick DB checks.

---

## Part A - Database in Docker

1. **Create a project folder**

```
TutorMvcDbFirst/
  ├─ src/
  └─ db/
```

2. **Add the SQL schema** (save as `db/schema.sql`):

```sql
CREATE DATABASE TutorDb;
GO
USE TutorDb;
GO

CREATE TABLE Students (
    StudentId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PhoneNumber NVARCHAR(15)
);

CREATE TABLE Tutors (
    TutorId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Subject NVARCHAR(50) NOT NULL
);

CREATE TABLE Sessions (
    SessionId INT IDENTITY(1,1) PRIMARY KEY,
    StudentId INT NOT NULL,
    TutorId INT NOT NULL,
    SessionDate DATETIME NOT NULL,
    DurationMinutes INT NOT NULL,
    FOREIGN KEY (StudentId) REFERENCES Students(StudentId),
    FOREIGN KEY (TutorId) REFERENCES Tutors(TutorId)
);
```

3. **Create an init script** (save as `db/init.sql`) to run after SQL starts:

```sql
-- Ensure DB exists and apply schema if needed
IF DB_ID('TutorDb') IS NULL
BEGIN
    CREATE DATABASE TutorDb;
END
GO
USE TutorDb;
GO

-- Idempotent creates
IF OBJECT_ID('dbo.Students','U') IS NULL
BEGIN
    CREATE TABLE Students (
        StudentId INT IDENTITY(1,1) PRIMARY KEY,
        FullName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        PhoneNumber NVARCHAR(15)
    );
END

IF OBJECT_ID('dbo.Tutors','U') IS NULL
BEGIN
    CREATE TABLE Tutors (
        TutorId INT IDENTITY(1,1) PRIMARY KEY,
        FullName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        Subject NVARCHAR(50) NOT NULL
    );
END

IF OBJECT_ID('dbo.Sessions','U') IS NULL
BEGIN
    CREATE TABLE Sessions (
        SessionId INT IDENTITY(1,1) PRIMARY KEY,
        StudentId INT NOT NULL,
        TutorId INT NOT NULL,
        SessionDate DATETIME NOT NULL,
        DurationMinutes INT NOT NULL,
        FOREIGN KEY (StudentId) REFERENCES Students(StudentId),
        FOREIGN KEY (TutorId) REFERENCES Tutors(TutorId)
    );
END

-- Optional seed
IF NOT EXISTS (SELECT 1 FROM Students)
INSERT INTO Students (FullName, Email, PhoneNumber) VALUES
('Aisha Khan','aisha@example.com','0712345678'),
('Liam Smith','liam@example.com','0823456789');

IF NOT EXISTS (SELECT 1 FROM Tutors)
INSERT INTO Tutors (FullName, Email, Subject) VALUES
('Naledi Mokoena','naledi@example.com','Math'),
('Johan van Wyk','johan@example.com','Physics');
```

4. **Create `docker-compose.yml` at repo root**

```yaml
services:
  sql:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: tutor_sql
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong!Passw0rd
      - MSSQL_PID=Developer
    ports:
      - "1433:1433"
    volumes:
      - mssql_data:/var/opt/mssql
      - ./db/init.sql:/docker-entrypoint-initdb.d/01-init.sql
    healthcheck:
      test: ["CMD", "/opt/mssql-tools/bin/sqlcmd", "-S", "localhost", "-U", "sa", "-P", "YourStrong!Passw0rd", "-Q", "SELECT 1"]
      interval: 5s
      timeout: 3s
      retries: 20

  web:
    build:
      context: ./src
      dockerfile: Dockerfile
    container_name: tutor_web
    depends_on:
      sql:
        condition: service_healthy
    environment:
      - ASPNETCORE_URLS=http://+:8080
      - ConnectionStrings__TutorDb=Server=sql,1433;Database=TutorDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;
    ports:
      - "8080:8080"

volumes:
  mssql_data:
```

> **Checkpoint Q:** What hostname will your app use to reach SQL inside compose?
> *(Answer verbally before proceeding.)*

---

## Part B - Create MVC project (Visual Studio)

1. **In VS**: *Create a new project* → **ASP.NET Core Web App (Model-View-Controller)**

   * Project name: `Tutor.Web`
   * Location: `TutorMvcDbFirst/src`
   * Framework: **.NET 8**
   * Authentication: **None**

2. **Install NuGet packages** (Project → Manage NuGet Packages):

   * `Microsoft.EntityFrameworkCore.SqlServer`
   * `Microsoft.EntityFrameworkCore.Tools`
   * `Microsoft.EntityFrameworkCore.Design`

3. **Add a connection string** to `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "TutorDb": "Server=localhost,1433;Database=TutorDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
  }
}
```

> Note: When running under Docker Compose, the connection will be injected via environment variable `ConnectionStrings__TutorDb`, so you don’t edit this file for container runs.

---

## Part C — DB‑First reverse engineering (scaffold)

**Option A — Package Manager Console (VS):**
Tools → NuGet Package Manager → **Package Manager Console**:

```powershell
Scaffold-DbContext `
 "Server=localhost,1433;Database=TutorDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;" `
 Microsoft.EntityFrameworkCore.SqlServer `
 -OutputDir Models `
 -Context TutorContext `
 -DataAnnotations `
 -UseDatabaseNames `
 -NoPluralize
```

**Option B — dotnet‑ef (terminal in `src/Tutor.Web`):**

```bash
dotnet tool install --global dotnet-ef
dotnet ef dbcontext scaffold "Server=localhost,1433;Database=TutorDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer \
  --output-dir Models --context TutorContext --data-annotations --use-database-names --no-pluralize
```

You should see `Models/TutorContext.cs` and entity classes for `Students`, `Tutors`, `Sessions`.

> **Mini‑check:** Which attribute enforces required fields on the model? *(Answer, then continue.)*

---

## Part D — Scaffold MVC CRUD

1. **Add Controllers**
   Right‑click *Controllers* → **Add → New Scaffolded Item → MVC Controller with views, using EF**

* Model class: `Student` → Data context: `TutorContext` → Name `StudentsController`
  Repeat for `Tutor` → `TutorsController`, and `Session` → `SessionsController`.

2. **Make Session create/edit use dropdowns**
   Open `SessionsController.cs` and modify `Create` (GET) to populate dropdowns:

```csharp
public IActionResult Create()
{
    ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FullName");
    ViewData["TutorId"] = new SelectList(_context.Tutors, "TutorId", "FullName");
    return View();
}
```

Ensure the `Create.cshtml` for Sessions uses the `asp-items` helpers:

```html
<div class="form-group">
    <label asp-for="StudentId"></label>
    <select asp-for="StudentId" class="form-control" asp-items="ViewBag.StudentId"></select>
    <span asp-validation-for="StudentId" class="text-danger"></span>
</div>
<div class="form-group">
    <label asp-for="TutorId"></label>
    <select asp-for="TutorId" class="form-control" asp-items="ViewBag.TutorId"></select>
    <span asp-validation-for="TutorId" class="text-danger"></span>
</div>
```

3. **Show related names on index/details**
   In `SessionsController.Index`, include relationships:

```csharp
var sessions = _context.Sessions
    .Include(s => s.Student)
    .Include(s => s.Tutor);
return View(await sessions.ToListAsync());
```

> **Mini‑check:** Why do we call `.Include(...)` here?

---

## Part E — Dockerize the MVC app

1. **Add `Dockerfile` in `src`** (next to the `.sln`? No — inside `src/Tutor.Web/`):

```dockerfile
# src/Tutor.Web/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "./Tutor.Web.csproj"
RUN dotnet publish "./Tutor.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Tutor.Web.dll"]
```

2. **Build & run with compose** (from repo root where `docker-compose.yml` is):

```bash
docker compose build
docker compose up
```

Browse to **[http://localhost:8080](http://localhost:8080)**.

---

## Part F — Test tasks (what students must demonstrate)

1. Create a **Student**, a **Tutor**, then a **Session** linking them.
2. Verify you **cannot** create two Students with the same email (unique constraint).
3. On **Sessions Index**, confirm Student and Tutor full names appear.
4. Edit a Session’s duration and confirm the change persists.
5. Stop and restart containers; confirm data persisted (thanks to the volume).

---

## Deliverables

* GitHub repo with:

  * `db/` folder (schema and init).
  * `src/Tutor.Web/` MVC app.
  * `Dockerfile` and `docker-compose.yml`.
* Short README with **how to run**, screenshots of CRUD pages, and a paragraph reflecting on DB‑First vs Code‑First.

---

