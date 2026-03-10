using FanDuel.DepthCharts.Extensions;
using FanDuel.DepthCharts.Helpers;
using FanDuel.DepthCharts.Models;
using FanDuel.DepthCharts.Models.Positions;
using FanDuel.DepthCharts.Repositories;
using FanDuel.DepthCharts.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddDepthChartServices();
var sp = services.BuildServiceProvider();

var chartFactory = sp.GetRequiredService<IDepthChartFactory>();
var repo = sp.GetRequiredService<IPlayerRepository>();
var depthChart = chartFactory.CreateChart<NflPosition>("TAM");

if (depthChart == null)
{
    Console.WriteLine("Failed to create depth chart.");
    return;
}

Player? GetPlayer(long id) => repo.GetById(id);

// Sample players from the challenge
var tomBrady = new Player(12, "Tom Brady");
var blaineGabbert = new Player(11, "Blaine Gabbert");
var kyleTrask = new Player(2, "Kyle Trask");
var mikeEvans = new Player(13, "Mike Evans");
var jaelonDarden = new Player(1, "Jaelon Darden");
var scottMiller = new Player(10, "Scott Miller");

// Populate repository so formatter can resolve ids to names
repo.Create(tomBrady);
repo.Create(blaineGabbert);
repo.Create(kyleTrask);
repo.Create(mikeEvans);
repo.Create(jaelonDarden);
repo.Create(scottMiller);

// Add players as per sample
depthChart.AddPlayerToDepthChart(NflPosition.QB, tomBrady.Number, 0);
depthChart.AddPlayerToDepthChart(NflPosition.QB, blaineGabbert.Number, 1);
depthChart.AddPlayerToDepthChart(NflPosition.QB, kyleTrask.Number, 2);
depthChart.AddPlayerToDepthChart(NflPosition.LWR, mikeEvans.Number, 0);
depthChart.AddPlayerToDepthChart(NflPosition.LWR, jaelonDarden.Number, 1);
depthChart.AddPlayerToDepthChart(NflPosition.LWR, scottMiller.Number, 2);

Console.WriteLine("\n=== getFullDepthChart() ===");
foreach (var line in DepthChartFormat.ToChart(ToStringKeyed(depthChart.GetFullDepthChart()), GetPlayer))
    Console.WriteLine(line);

Console.WriteLine("\n=== getBackups(\"QB\", Tom Brady) ===");
foreach (var line in DepthChartFormat.FormatPlayerIdLines(depthChart.GetBackups(NflPosition.QB, tomBrady.Number), GetPlayer))
    Console.WriteLine(line);

Console.WriteLine("\n=== getBackups(\"LWR\", Jaelon Darden) ===");
foreach (var line in DepthChartFormat.FormatPlayerIdLines(depthChart.GetBackups(NflPosition.LWR, jaelonDarden.Number), GetPlayer))
    Console.WriteLine(line);

Console.WriteLine("\n=== getBackups(\"QB\", Mike Evans) ===");
foreach (var line in DepthChartFormat.FormatPlayerIdLines(depthChart.GetBackups(NflPosition.QB, mikeEvans.Number), GetPlayer))
    Console.WriteLine(line);

Console.WriteLine("\n=== getBackups(\"QB\", Blaine Gabbert) ===");
foreach (var line in DepthChartFormat.FormatPlayerIdLines(depthChart.GetBackups(NflPosition.QB, blaineGabbert.Number), GetPlayer))
    Console.WriteLine(line);

Console.WriteLine("\n=== getBackups(\"QB\", Kyle Trask) ===");
foreach (var line in DepthChartFormat.FormatPlayerIdLines(depthChart.GetBackups(NflPosition.QB, kyleTrask.Number), GetPlayer))
    Console.WriteLine(line);

Console.WriteLine("\n=== getFullDepthChart() ===");
foreach (var line in DepthChartFormat.ToChart(ToStringKeyed(depthChart.GetFullDepthChart()), GetPlayer))
    Console.WriteLine(line);

Console.WriteLine("\n=== removePlayerFromDepthChart(\"LWR\", Mike Evans) ===");
var removed = depthChart.RemovePlayerFromDepthChart(NflPosition.LWR, mikeEvans.Number);
var removedLines = DepthChartFormat.FormatPlayerIdLines(removed, GetPlayer).ToList();
Console.WriteLine(removedLines.Count > 0 ? string.Join(Environment.NewLine, removedLines) : "<NO LIST>");

Console.WriteLine("\n=== getFullDepthChart() after removal ===");
foreach (var line in DepthChartFormat.ToChart(ToStringKeyed(depthChart.GetFullDepthChart()), GetPlayer))
    Console.WriteLine(line);

static IReadOnlyDictionary<string, IReadOnlyList<long>> ToStringKeyed(IReadOnlyDictionary<NflPosition, IReadOnlyList<long>> chart)
{
    var d = new Dictionary<string, IReadOnlyList<long>>();
    foreach (var (pos, ids) in chart)
        d[pos.ToString()] = ids;
    return d;
}
