# 🗄️ WebDBA - Web Database Administrator

**WebDBA** — это веб-приложение для администрирования и управления структурными подразделениями и сотрудниками. Система построена на архитектуре клиент-сервер с разделением на три независимых компонента.

---

## 📦 Структура решения

Решение состоит из трёх проектов:

| Проект | Назначение |
|--------|------------|
| **WebDBA** | Веб-приложение (клиентская часть) — UI для работы с подразделениями и сотрудниками |
| **WebDBA.API** | API — программный интерфейс для взаимодействия с базой данных |
| **WebDBA.Migrator** | Миграции — управление схемой базы данных |

---

## 🛠 Технологический стек

- **.NET 9.0**
- **ASP.NET Core MVC** (WebDBA)
- **ASP.NET Core Web API** (WebDBA.API)
- **Entity Framework Core** (ORM)
- **PostgreSQL** (БД)
- **Npgsql** (PostgreSQL 15)
- **Bootstrap 5** + **Bootstrap Icons** (UI)
- **JavaScript** + **jQuery** (клиентская логика)

---

## ⚙️ Настройка проекта

### 1. Строка подключения к БД

В файле `WebDBA.API/appsettings.json` укажите строку подключения к PostgreSQL:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7099"
  }
}
```

### 2. Адрес API

в файле `WebDBA/appsettings.json` укажите URL, где запущен API:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7099",
    "TimeoutSeconds": 30,
    "AcceptHeader": "application/json"
  }
}
```
