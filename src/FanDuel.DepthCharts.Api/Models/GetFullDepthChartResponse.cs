namespace FanDuel.DepthCharts.Api.Models;

/// <summary>Response containing the full depth chart (position to ordered player IDs).</summary>
public record GetFullDepthChartResponse(IReadOnlyDictionary<string, IReadOnlyList<long>> Positions);
