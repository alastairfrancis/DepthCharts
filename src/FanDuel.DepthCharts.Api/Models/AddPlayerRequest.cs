namespace FanDuel.DepthCharts.Api.Models;

/// <summary>Request body for adding a player to a depth chart.</summary>
public record AddPlayerRequest(string Position, long PlayerId, int? PositionDepth = null);
