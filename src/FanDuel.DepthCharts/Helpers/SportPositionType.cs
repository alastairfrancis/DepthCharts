using FanDuel.DepthCharts.Models;
using FanDuel.DepthCharts.Models.Positions;

namespace FanDuel.DepthCharts.Helpers;

/// <summary>
/// Position type mappings for each sport.
/// </summary>
public class SportPositionType
{
    private static readonly Dictionary<Sport, Type> SportPositionMap = new()
    {
        [Sport.NFL] = typeof(NflPosition),
        [Sport.MLB] = typeof(MlbPosition),
        [Sport.NHL] = typeof(NhlPosition),
        [Sport.NBA] = typeof(NbaPosition)
    };

    /// <summary>
    /// Gets the position enum type for the given sport (e.g. NflPosition for NFL).
    /// </summary>
    /// <returns>The type, or null if not valid.</returns>
    public static Type? GetPositionType(Sport sport) => SportPositionMap.TryGetValue(sport, out var t) ? t : null;
}
