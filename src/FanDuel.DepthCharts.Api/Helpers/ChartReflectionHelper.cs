using System.Collections;
using System.Reflection;

namespace FanDuel.DepthCharts.Api.Helpers;

/// <summary>
/// Invokes IDepthChart methods via reflection when the position type is only known at runtime (Api layer).
/// </summary>
internal static class ChartReflectionHelper
{
    public static void AddPlayerToDepthChart(object chart, string position, long playerId, int? positionDepth)
    {
        var method = chart.GetType().GetMethod("AddPlayerToDepthChart", [typeof(string), typeof(long), typeof(int?)])
            ?? throw new InvalidOperationException("AddPlayerToDepthChart(string, long, int?) not found.");
        method.Invoke(chart, [position, playerId, positionDepth]);
    }

    public static IReadOnlyDictionary<string, IReadOnlyList<long>> GetFullDepthChartStringKeyed(object chart)
    {
        var method = chart.GetType().GetMethod("GetFullDepthChart", Type.EmptyTypes)
            ?? throw new InvalidOperationException("GetFullDepthChart not found.");
        var result = method.Invoke(chart, null);
        if (result == null)
            return new Dictionary<string, IReadOnlyList<long>>();

        var dict = (IDictionary)result;
        var stringKeyed = new Dictionary<string, IReadOnlyList<long>>();
        foreach (DictionaryEntry entry in dict)
            stringKeyed[entry.Key.ToString()!] = (IReadOnlyList<long>)entry.Value!;
        return stringKeyed;
    }

    public static IReadOnlyList<long> GetBackups(object chart, string position, long playerId)
    {
        var method = chart.GetType().GetMethod("GetBackups", [typeof(string), typeof(long)])
            ?? throw new InvalidOperationException("GetBackups(string, long) not found.");
        var result = method.Invoke(chart, [position, playerId]);
        return (IReadOnlyList<long>)result!;
    }
}
