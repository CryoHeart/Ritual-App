# RITUAL

A full-stack web application for bands to manage their songs, albums, setlists, and live performance sessions.

---

## Tech Stack

### Frontend
| Tool | Version | Purpose |
|---|---|---|
| [React](https://react.dev/) | 19 | UI framework |
| [TypeScript](https://www.typescriptlang.org/) | ~6.0 | Type-safe JavaScript |
| [Vite](https://vite.dev/) | 8 | Build tool & dev server |
| [Tailwind CSS](https://tailwindcss.com/) | 4 | Utility-first CSS framework |
| [ESLint](https://eslint.org/) | 10 | Linting |

### Backend
| Tool | Version | Purpose |
|---|---|---|
| [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet) | 8.0 | Web API framework |
| [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) | 8.0 | ORM |
| [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql) | 8.0.2 | MySQL EF Core provider |
| [BCrypt.Net-Next](https://github.com/BcryptNet/bcrypt.net) | 4.0.3 | Password hashing |
| [Swashbuckle / Swagger](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) | 6.5.0 | API documentation |

### Database
| Tool | Purpose |
|---|---|
| [MySQL](https://www.mysql.com/) 8.0 | Relational database |

---

## Features

- **Authentication** — Band registration and login with secure BCrypt password hashing
- **Band Account Management** — Manage band profile and settings
- **Song Management** — Create, edit, and organize your song catalog
- **Albums** — Group songs into albums
- **Setlist Editor** — Build and manage setlists from your song catalog
- **Ceremony Mode** — Live performance view with real-time setlist progress tracking
- **Dashboard** — Overview of band stats and activity

---

## Project Structure

```
RITUAL/
├── client/          # React + Vite frontend (port 5173)
├── server/          # ASP.NET Core 8 Web API (port 5000)
├── db/              # Database schema SQL
├── migrations/      # SQL migration files
└── RITUAL.sln       # Visual Studio solution
```

### Frontend (`client/src/`)
```
api/          # HTTP client and API modules per resource
components/   # Reusable UI components
context/      # React context (AuthContext)
pages/        # Page-level components
types/        # TypeScript type definitions
```

### Backend (`server/`)
```
Controllers/  # REST API controllers
Dao/          # Data Access Objects (interfaces + implementations)
Data/         # EF Core DbContext and entity configurations
Logic/        # Business logic layer (interfaces + implementations)
Models/       # Request and response DTOs
```

---

## API Endpoints

| Resource | Base Route |
|---|---|
| Auth | `POST /api/auth/login`, `POST /api/auth/register` |
| Bands | `/api/bands` |
| Songs | `/api/bands/{bandId}/songs` |
| Albums | `/api/bands/{bandId}/albums` |
| Setlists | `/api/bands/{bandId}/setlists` |
| Live Sessions | `/api/bands/{bandId}/live-sessions` |
| Health | `/api/health` |

Full interactive API docs available via Swagger UI at `http://localhost:5000/swagger` when running in development.

---

## Getting Started

### Prerequisites

- [Node.js](https://nodejs.org/) (LTS recommended)
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [MySQL 8.0](https://dev.mysql.com/downloads/)

### Database Setup

1. Create the database schema (local MySQL only — skip if using a hosted provider such as Aiven):
   ```sql
   -- Run the base schema
   SOURCE db/ritual_db.sql;

   -- Apply migrations in order
   SOURCE migrations/001_add_albums.sql;
   SOURCE migrations/002_song_editing_schema.sql;
   SOURCE migrations/003_seed_wolfs_cry_album.sql;
   ```

2. Configure the database connection via environment variables.
   Copy `server/.env.example` to `server/.env` and fill in your values:
   ```
   cp server/.env.example server/.env
   ```
   Then edit `server/.env`:
   ```env
   DB_HOST=your-mysql-host
   DB_PORT=3306
   DB_USER=your_user
   DB_PASSWORD=your_password
   DB_NAME=ritual
   DB_SSL=false
   ```
   For hosted databases such as [Aiven](https://aiven.io/), set `DB_SSL=true` and use the provided host and port.

   > **Never commit `server/.env`** — it is listed in `.gitignore` and must remain local only.

### Running the Backend

```bash
cd server
dotnet run
```

The API will be available at `http://localhost:5000`.

### Running the Frontend

```bash
cd client
npm install
npm run dev
```

The client will be available at `http://localhost:5173`.

---

## Architecture

RITUAL follows a layered architecture on the backend:

```
Controller → Logic (Business Layer) → DAO (Data Access Layer) → EF Core → MySQL
```

- **Controllers** handle HTTP routing and request/response mapping
- **Logic** layer enforces business rules
- **DAO** layer abstracts all database queries
- **EF Core** manages object-relational mapping via `RitualDbContext`
