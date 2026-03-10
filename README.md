# FanDuel Trading Solutions – Depth Charts challenge

A .NET 9 C# solution that implements depth chart management for sports. 
It supports adding/removing players by position, querying backups, and retrieving the full depth chart.

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

---

## Assumptions and notes

1. **Player identity**  
  Players are identified by numeric id within the chart. The same player id can appear at multiple positions. The player details are assumed to be stored elsewhere.  I feel it's unwise to duplicate information like player names as it could quickly become inconsistent with other parts of the system.

2. **Multiple charts**  
  The system supports multiple charts (e.g. different teams, sports) via the `IDepthChartFactory`. Each chart for a team is independent.

3. **In-memory storage**
  The depth charts are stored in-memory for simplicity. In a real system, we would likely want to persist this data in a database or distributed cache (e.g. Redis).

4. **Minimal change functionality**  
  In a distributed system, we would need to support more complex operations like player or team deletion, or players moving teams.

5. **Additional validation**
  The current implementation assumes valid player ids are provided. We would need to check that player is a member of the team, but that responsibility likely belongs to another service that manages team, and player information.
---

## Solution layout

| Project | Purpose |
|--------|---------|
| `src/FanDuel.DepthCharts` | Core library: `IDepthChart`, `ISportDepthChartFactory`, `Sport`, add/remove/backups/full chart. **Packaged as NuGet for use in other projects.** |
| `src/FanDuel.DepthCharts.Console` | Console app that runs the challenge sample (add, backups, full chart, remove). |
| `src/FanDuel.DepthCharts.Api` | ASP.NET Core Web API with Swagger; create chart, add player, get full chart, get backups. |
| `tests/FanDuel.DepthCharts.Tests` | xUnit tests for depth chart behavior and edge cases. |

---

## Build (entire solution)

From the solution root:

```bash
dotnet restore
dotnet build
```
---

## Run unit tests

From the solution root:

```bash
dotnet test
```

The test project covers backup lists, full depth chart order, add/remove behavior, multiple positions, and edge cases (null position, player not on chart, etc.).

---

## Start the API server

From the solution root:

```bash
dotnet run --project src/FanDuel.DepthCharts.Api/FanDuel.DepthCharts.Api.csproj
```


The API listens on the URLs shown in the console (for non-default ports modify `launchSettings.json`).

---

## Swagger URL

When the API is running, open Swagger UI in your browser:

- **Swagger UI:** `https://localhost:<port>/swagger`  

---

## Including the FanDuel.DepthCharts library in other projects

The **FanDuel.DepthCharts** library is intended to be **built using NuGet** 
and **included in other solutions as a NuGet package**.

### Building the library as a NuGet package

From the solution root, pack the library project:

```bash
dotnet pack src/FanDuel.DepthCharts/FanDuel.DepthCharts.csproj -c Release -o ./nupkgs
```

This produces a `.nupkg` file.

### Adding the library to another project via NuGet

In the project where you want to use depth charts, **add the package reference**:

**PackageReference (e.g. in a `.csproj`):**

```xml
<ItemGroup>
  <PackageReference Include="FanDuel.DepthCharts" Version="1.0.0" />
</ItemGroup>
```

**Or via .NET CLI:**

Then register the library services and use the factory in your app:

```csharp
using FanDuel.DepthCharts;
using FanDuel.DepthCharts.Models;
using FanDuel.DepthCharts.Services;

// In Startup / Program.cs (or wherever you configure services)
services.AddDepthChartServices();

// Resolve ISportDepthChartFactory and use it
var factory = app.Services.GetRequiredService<ISportDepthChartFactory>();
IDepthChart chart = factory.GetOrCreateChart(Sport.NFL, "TB");
chart.AddPlayerToDepthChart("QB", 12, 0);
var backups = chart.GetBackups("QB", 12);
var fullChart = chart.GetFullDepthChart();
```

---

## Run the console sample

A console app is included that demonstrates the library usage with the sample data from the challenge description. 
To run it:

```bash
dotnet run --project src/FanDuel.DepthCharts.Console/FanDuel.DepthCharts.Console.csproj
```

---