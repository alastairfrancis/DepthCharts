namespace FanDuel.DepthCharts.Services;

/// <summary>
/// Factory for depth charts per team.
/// </summary>
public interface IDepthChartFactory
{
    /// <summary>
    /// Creates the depth chart for the given team.
    /// </summary>
    /// <typeparam name="TPosition">Sport-specific position enum (e.g. NflPosition, MlbPosition).</typeparam>
    /// <returns>The created depth chart, or null if a chart already exists.</returns>
    IDepthChart<TPosition>? CreateChart<TPosition>(string teamId) where TPosition : struct, Enum;

    /// <summary>
    /// Gets an existing typed chart if one has been created for the given team.
    /// </summary>
    /// <returns>The depth chart, or null if a chart does not exist.</returns>
    IDepthChart<TPosition>? GetChart<TPosition>(string teamId) where TPosition : struct, Enum;

    /// <summary>
    /// Gets an existing typed chart, or creates a new one if it doesn't exist.
    /// </summary>
    /// <returns>The depth chart, or null if a chart does not exist.</returns>
    IDepthChart<TPosition>? GetOrCreateChart<TPosition>(string teamId) where TPosition : struct, Enum;
}
