using Microsoft.Extensions.Logging;

namespace FanDuel.DepthCharts;

/// <summary>
/// Manages a single team's depth chart with strongly-typed positions.
/// Stores only player ids. Full player data can be resolved via the repository.
/// </summary>
public class DepthChart<TPosition>(ILogger<DepthChart<TPosition>> logger) : IDepthChart<TPosition>
    where TPosition : struct, Enum
{
    private readonly Lock _sync = new();
    private readonly Dictionary<TPosition, List<long>> _chart = [];
    private readonly ILogger<DepthChart<TPosition>> _logger = logger;

    public bool AddPlayerToDepthChart(string position, long playerId, int? positionDepth = null)
    {
        var pos = ConvertToPosition(position);
        if (pos is null)
        {
            return false;
        }

        return AddPlayerToDepthChart(pos.Value, playerId);
    }

    public bool AddPlayerToDepthChart(TPosition position, long playerId, int? positionDepth = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(playerId);

        int index;

        lock (_sync)
        {
            if (!_chart.TryGetValue(position, out var list))
            {
                list = [];
                _chart[position] = list;
            }

            // Check if player is already in the list for this position.
            if (list.Contains(playerId))
            {
                return false;
            }

            // If positionDepth is not provided, add to the end of the list
            index = positionDepth.HasValue ? Math.Clamp(positionDepth.Value, 0, list.Count) : list.Count;
            if (index < list.Count)
            {
                list.Insert(index, playerId);
            }
            else
            {
                list.Add(playerId);
            }
        }

        _logger.LogInformation(
            "{Name}: player added. PlayerId={PlayerId}, Position={Position}, DepthIndex={DepthIndex}",
            nameof(AddPlayerToDepthChart), playerId, position, index);

        return true;
    }

    public IReadOnlyList<long> RemovePlayerFromDepthChart(string position, long playerId)
    {
        var pos = ConvertToPosition(position);
        if (pos is null)
        {
            return [];
        }

        return RemovePlayerFromDepthChart(pos.Value, playerId);
    }

    public IReadOnlyList<long> RemovePlayerFromDepthChart(TPosition position, long playerId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(playerId);

        lock (_sync)
        {
            if (!_chart.TryGetValue(position, out var list))
            {
                _logger.LogInformation("{Name}: position {Position} not found", nameof(RemovePlayerFromDepthChart), position);
                return [];
            }

            int idx = list.IndexOf(playerId);
            if (idx < 0)
            {
                _logger.LogInformation("{Name}: PlayerId={PlayerId} not at position {Position}", nameof(RemovePlayerFromDepthChart), playerId, position);
                return [];
            }

            list.RemoveAt(idx);
            return [playerId];
        }
    }

    public IReadOnlyList<long> GetBackups(string position, long playerId)
    {
        var pos = ConvertToPosition(position);
        if (pos is null)
        {
            return [];
        }

        return GetBackups(pos.Value, playerId);
    }

    public IReadOnlyList<long> GetBackups(TPosition position, long playerId)
    {
        lock (_sync)
        {
            if (!_chart.TryGetValue(position, out var list))
                return [];

            int index = list.IndexOf(playerId);
            if (index < 0 || index == list.Count - 1)
                return [];

            return [.. list.Skip(index + 1)];
        }
    }

    public IReadOnlyDictionary<TPosition, IReadOnlyList<long>> GetFullDepthChart()
    {
        lock (_sync)
        {
            var result = new Dictionary<TPosition, IReadOnlyList<long>>();

            // TODO: consider the order these are returned. Should we sort by position enum order, or alphabetical?
            foreach (var pos in _chart.Keys)
            {
                if (_chart.TryGetValue(pos, out var ids))
                {
                    result[pos] = [.. ids];
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Convert string to TPosition type
    /// </summary>
    /// <returns>null if position is not valid TPosition type</returns>
    private static TPosition? ConvertToPosition(string position)
    {
        if (!string.IsNullOrWhiteSpace(position))
        {
            try
            {
                return Enum.Parse<TPosition>(position, ignoreCase: true);
            }
            catch (ArgumentException)
            { }
            catch (InvalidOperationException)
            { }
        }

        return null;
    }
}
