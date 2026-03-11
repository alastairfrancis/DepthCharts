# FanDuel Trading Solutions – Depth Charts

A .NET C# solution for managing depth charts: add/remove players by position, query backups, and retrieve the full chart. Supports multiple teams via `IDepthChartFactory` (keyed by `teamId`).

---

## Table of contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Design notes](#design-notes)
- [Quick start](#quick-start)
- [Solution layout](#solution-layout)
- [Build and test](#build-and-test)
- [API](#api)
- [Using the library](#using-the-fandueldepthcharts-library)

---

## Features

- **Depth chart operations**: Add player at position (with optional depth), remove player, get backups for a player, get full chart.
- **Typed positions**: Use sport-specific enums (e.g. `NflPosition`, `MlbPosition`) or string positions where the type is not known at compile time.
- **Per-team charts**: `IDepthChartFactory` creates or returns a chart per `teamId`; charts are independent.
- **API and console**: ASP.NET Core Web API with Swagger, plus a console sample that runs the challenge scenario.

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

---

## Design notes

1. **Player identity**  
   Players are identified by numeric id in the chart. The same player id can appear at multiple positions. Player details (e.g. names) are assumed to live in another store (e.g. `IPlayerRepository`) to avoid duplicating and inconsistent data.

2. **Multiple charts**  
   Charts are per team, keyed by `teamId` via `IDepthChartFactory`.

3. **In-memory storage**  
   Depth charts are stored in-memory. In production you would typically persist to a database or distributed cache (e.g. Redis).

4. **Scope**  
   The implementation focuses on depth chart operations. A full system would also handle team/player event updates (e.g. player or team deletion, players moving teams).

5. **Validation**  
   The library assumes valid player ids and that the caller (or another service) ensures players belong to the team. Team and player membership would be maintained by other systems.

6. **Sample API**  
   A sample API is provided. If the library were containerized, it could expose depth chart services to other systems. The API requires `sport` and `teamId` in the URL because I don't have enough team data to infer the sport.

---

## Quick start

```bash
# Clone and open the repo, then from the solution root:
dotnet restore
dotnet build
dotnet test

# Run the API (then open Swagger at https://localhost:<port>/swagger)
dotnet run --project src/FanDuel.DepthCharts.Api/FanDuel.DepthCharts.Api.csproj

# Or run the console sample
dotnet run --project src/FanDuel.DepthCharts.Console/FanDuel.DepthCharts.Console.csproj
```

---

## Solution layout

| Project | Purpose |
|--------|---------|
| `src/FanDuel.DepthCharts` | Core library: `IDepthChart<TPosition>`, `IDepthChartFactory`, position enums, add/remove/backups/full chart. Can be packed as a NuGet package. |
| `src/FanDuel.DepthCharts.Console` | Console app: runs the challenge sample (add players, full chart, backups, remove). |
| `src/FanDuel.DepthCharts.Api` | ASP.NET Core Web API with Swagger: create chart, add player, get full chart, get backups. |
| `tests/FanDuel.DepthCharts.Tests` | xUnit tests: depth chart behavior, factory, repository, edge cases. |

---

## Build and test

From the solution root:

```bash
dotnet restore
dotnet build
dotnet test
```

Tests cover backup lists, full depth chart order, add/remove behavior, duplicate players, multiple positions, and edge cases (player not on chart, unknown position, etc.).

---

## API

Base route: `api/sports/{sport}/teams/{teamId}`. Supported `sport` values depend on `SportPositionType` (e.g. NFL, MLB, NHL, NBA).

**Swagger UI:** when the API is running, open `https://localhost:<port>/swagger` in your browser.

---

## Run the console sample

The console app demonstrates the library with the challenge sample data:

```bash
dotnet run --project src/FanDuel.DepthCharts.Console/FanDuel.DepthCharts.Console.csproj
```

---

## Using the FanDuel.DepthCharts library

The core library can be packed as NuGet and consumed from other solutions.

### Pack the library

From the solution root:

```bash
dotnet pack src/FanDuel.DepthCharts/FanDuel.DepthCharts.csproj -c Release -o ./nupkgs
```

### Reference the package

**PackageReference (in `.csproj`):**

```xml
<ItemGroup>
  <PackageReference Include="FanDuel.DepthCharts" Version="1.0.0" />
</ItemGroup>
```

**Or via .NET CLI** (from the consuming project directory):

```bash
dotnet add package FanDuel.DepthCharts --version 1.0.0
```

### Register services and use the factory

Charts are keyed by `teamId` only. Use the generic methods with a position enum type (e.g. `NflPosition`) or call `GetOrCreateChart<TPosition>(teamId)` via reflection when the position type is known only at runtime.

```csharp
using FanDuel.DepthCharts.Extensions;
using FanDuel.DepthCharts.Models;
using FanDuel.DepthCharts.Models.Positions;
using FanDuel.DepthCharts.Services;

// Register library services (e.g. in Program.cs or Startup)
services.AddDepthChartServices();

// Resolve the factory and get or create a chart by teamId
var factory = app.Services.GetRequiredService<IDepthChartFactory>();
var chart = factory.GetOrCreateChart<NflPosition>("TB");
if (chart != null)
{
    chart.AddPlayerToDepthChart(NflPosition.QB, 12, 0);
    var backups = chart.GetBackups(NflPosition.QB, 12);
    var fullChart = chart.GetFullDepthChart();
}
```
---
