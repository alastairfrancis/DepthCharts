namespace FanDuel.DepthCharts.Api.Models;

/// <summary>Response containing backup player IDs for a position and player.</summary>
public record GetBackupsResponse(IReadOnlyList<long> PlayerIds);
