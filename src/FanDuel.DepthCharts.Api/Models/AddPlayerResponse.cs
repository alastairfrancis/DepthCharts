namespace FanDuel.DepthCharts.Api.Models;

/// <summary>Response after adding a player to a depth chart.</summary>
public record AddPlayerResponse(string Sport, string TeamId, string Position, long PlayerId, int? PositionDepth);
