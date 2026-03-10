using FanDuel.DepthCharts.Models;

namespace FanDuel.DepthCharts.Helpers;

/// <summary>
/// Standalone formatting helper for depth chart display. No dependencies; pass a player resolver when formatting ids.
/// </summary>
public static class DepthChartFormat
{
    /// <summary>
    /// Formats a single position line: "QB – (#12, Tom Brady), (#11, Blaine Gabbert)".
    /// </summary>
    public static string ToChartLine(string position, IReadOnlyList<long> playerIds, Func<long, Player?> getPlayer)
    {
        if (string.IsNullOrEmpty(position))
            return string.Empty;
        if (playerIds == null || playerIds.Count == 0)
            return $"{position} – ";

        var parts = playerIds
            .Select(id => getPlayer(id))
            .Where(p => p != null)
            .Select(p => $"(#{p!.Number}, {p.Name})");
        var segment = string.Join(", ", parts);
        return $"{position} – {segment}";
    }

    /// <summary>
    /// Formats a full depth chart as one line per position.
    /// </summary>
    public static IEnumerable<string> ToChart(
        IReadOnlyDictionary<string, IReadOnlyList<long>> depthChart,
        Func<long, Player?> getPlayer)
    {
        if (depthChart == null || depthChart.Count == 0)
            yield break;

        foreach (var (position, playerIds) in depthChart)
            yield return ToChartLine(position, playerIds, getPlayer);
    }

    /// <summary>
    /// Formats a list of player ids as one line per player (#number – name). Yields "&lt;NO LIST&gt;" if empty.
    /// </summary>
    public static IEnumerable<string> FormatPlayerIdLines(IReadOnlyList<long> playerIds, Func<long, Player?> getPlayer)
    {
        if (playerIds == null || playerIds.Count == 0)
        {
            yield return "<NO LIST>";
            yield break;
        }

        foreach (var id in playerIds)
        {
            var p = getPlayer(id);
            yield return p != null ? $"#{p.Number} – {p.Name}" : $"#{id}";
        }
    }
}
