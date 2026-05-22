# 🎮 Game Launcher

Modern desktop game launcher built with .NET MAUI following the MVVM architectural pattern.

The application provides a centralized platform for browsing games, managing personal libraries, publishing reviews, interacting with other users, and organizing game-related content through a clean multi-layered architecture.

## Features

-  Browse games in the Store
-  Manage personal game library
-  Create and edit reviews
-  User profiles and social features
-  Search, filtering and sorting
-  Persistent SQLite database storage
-  CRUD operations across all major entities

## Architecture

The project follows a layered architecture:

```text
App (MAUI UI)
    ↓
Business Logic Layer (BL)
    ↓
Data Access Layer (DAL)
    ↓
SQLite Database
```

The frontend is implemented using the MVVM pattern:

```text
View
    ↓
ViewModel
    ↓
Facade
    ↓
Repository
    ↓
DbContext
    ↓
SQLite
```

## Technology Stack

- C#
- .NET 10
- .NET MAUI
- Entity Framework Core
- SQLite
- CommunityToolkit.Mvvm
- Dependency Injection
- xUnit
- Azure DevOps

## Project Structure

```text
Launcher.App          MAUI frontend
Launcher.BL           Business Logic Layer
Launcher.DAL          Data Access Layer
Launcher.BL.Tests     Business layer tests
Launcher.DAL.Tests    Data layer tests
```

## Running the Application

### Prerequisites

- .NET 10 SDK
- MAUI Workloads installed

### Build

```bash
dotnet build
```

### Run

```bash
dotnet run --project Launcher.App
```

### Tests

```bash
dotnet test
```

## Design Patterns

- MVVM
- Dependency Injection
- Repository
- Facade
- Unit of Work (via EF Core DbContext)

## Authors
Developed as a team project during the *C# Seminar* course at FIT VUT.

Developed as a team project using Azure DevOps, Git, code reviews and pull request workflows.