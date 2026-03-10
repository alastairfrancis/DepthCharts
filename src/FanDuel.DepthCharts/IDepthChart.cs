namespace FanDuel.DepthCharts;

/// <summary>
/// Manages a single team's depth chart.
/// </summary>
/// <typeparam name="TPosition">Sport-specific position enum (e.g. NflPosition, MlbPosition).</typeparam>
public interface IDepthChart<TPosition> where TPosition : struct, Enum
{
    /// <summary>
    /// Adds a player to the depth chart at the given position.
    /// </summary>
    /// <param name="position">Position (e.g. QB, LWR).</param>
    /// <param name="playerId">The player to add.</param>
    /// <param name="positionDepth">Optional. Zero-based depth. If null, player is added at the end. Existing players at or below this depth are shifted down.</param>
    /// <returns>True if the player was added successfully; false if the player was already at the specified position and depth.</returns>
    bool AddPlayerToDepthChart(TPosition position, long playerId, int? positionDepth = null);
    bool AddPlayerToDepthChart(string position, long playerId, int? positionDepth = null);

    /// <summary>
    /// Removes a player from the depth chart at the given position.
    /// </summary>
    /// <param name="position">Position code.</param>
    /// <param name="playerId">The player to remove.</param>
    /// <returns>A list containing the removed player id, or an empty list if the player was not on the chart at that position.</returns>
    IReadOnlyList<long> RemovePlayerFromDepthChart(TPosition position, long playerId);
    IReadOnlyList<long> RemovePlayerFromDepthChart(string position, long playerId);

    /// <summary>
    /// Returns the full depth chart: every position and its ordered list of players.
    /// </summary>
    /// <returns>Position-to-players map. Positions in insertion order; players within each position in depth order.</returns>
    IReadOnlyDictionary<TPosition, IReadOnlyList<long>> GetFullDepthChart();

    /// <summary>
    /// For a given player and position, returns all players that are backups (lower depth / higher index).
    /// </summary>
    /// <param name="position">Position.</param>
    /// <param name="playerId">The player id at that position.</param>
    /// <returns>Backups in depth order. Empty if the player has no backups or is not at that position.</returns>
    IReadOnlyList<long> GetBackups(TPosition position, long playerId);
    IReadOnlyList<long> GetBackups(string position, long playerId);
}
