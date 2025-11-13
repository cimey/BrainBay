# BrainBay

BrainBay is a .NET 8-based application that fetches, stores, and displays characters from the **Rick and Morty** API.  
It consists of a **Web API** for browsing characters, a **console-based background worker** for syncing data, and uses **SQL Server / SQLite** for storage and **Redis** for caching.

---

## Features

- Fetch character data from the [Rick and Morty API](https://rickandmortyapi.com/api/character/)
- Store only alive characters in the database
- Web API to list and add new characters
- Cached character retrieval with Redis / MemoryCache
- Automatic cache invalidation when new characters are added
- Background worker that periodically syncs data from the API
- Input validation using FluentValidation
- Unit and integration tests
- Clean Architecture: separation of Core, Application, Infrastructure, and API layers

---

## Project Structure

BrainBay/
├─ BrainBay.API/ # Web API project
├─ BrainBay.Console.BackgroundWorker/ # Background worker project
├─ BrainBay.Application/ # Application layer (services, mapping, DI)
├─ BrainBay.Core/ # Domain entities and value objects
├─ BrainBay.Infrastructure/ # Database context, repositories, caching
├─ BrainBay.IntegrationTests/ # Integration tests
├─ BrainBay.UnitTests/ # Unit tests
├─ docker-compose.yml # Docker Compose for API, Worker, SQL Server, Redis


---

## Technologies & Packages

- **.NET 8**
- **C# 12**
- **Entity Framework Core 8** (SQL Server / SQLite)
- **FluentValidation** for input validation
- **AutoMapper** for entity ↔ DTO mapping
- **Redis / IMemoryCache** for caching
- **HttpClientFactory** (`Microsoft.Extensions.Http`) for API calls
- **Xunit** & **FluentAssertions** for unit and integration tests
- **Docker** and **Docker Compose** for containerization

---

## Setup Instructions

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Docker (for containerized setup)
- SQL Server or SQLite (if running locally)
- Redis (if running locally)

---

### Running Locally

1. Clone the repository:

```bash
git clone https://github.com/cimey/BrainBay.git
cd BrainBay
run below commands
docker-compose build
docker-compose up -d
- API: http://localhost:5000
- SQL Server: localhost,1433
- Redis: localhost:6379

| Endpoint               | Method | Description            |
| ---------------------- | ------ | ---------------------- |
| `/api/characters`      | GET    | Get list of characters |
| `/api/characters/{id}` | GET    | Get a single character |
| `/api/characters`      | POST   | Add a new character    |

Notes

Cached results are invalidated every 5 minutes or when a new character is added.

Both origin and location properties are optional but validated if provided.

Episodes must be valid URLs.
