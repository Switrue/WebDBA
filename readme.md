# 🗄️ WebDBA - Web Database Administrator

**WebDBA** is a web application for administering and managing business units and employees. The system is built on a client-server architecture divided into three independent components.

---

## 📦 Solution structure

The solution consists of three projects:

| Project | Destination |
|--------|-----------|
| **WebDBA** | Web application (client part) - UI for working with departments and employees |
| **WebDBA.API** | API - program interface for interacting with the database |
| **WebDBA.Migrator** | Migrations - Database Schema Management |

---

## 🛠 Technology stack

- **.NET 9.0**
- **ASP.NET Core MVC** (WebDBA)
- **ASP.NET Core Web API** (WebDBA.API)
- **Entity Framework Core** (ORM)
- **PostgreSQL** (DB)
- **Npgsql** (PostgreSQL 15)
- **Bootstrap 5** + **Bootstrap Icons** (UI)
- **JavaScript** + **jQuery** (client logic)

---

## ⚙️ Project setup

### 1. Database connection string

In the `WebDBA.API/appsettings.json` file, specify the PostgreSQL connection string:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7099"
  }
}
```

### 2. API address

in the `WebDBA/appsettings.json` file specify the URL where the API is running:

```json
{
  "ApiSettings": {
"BaseUrl": "https://localhost:7099",
    "TimeoutSeconds": 30,
    "AcceptHeader": "application/json"
  }
}
```
