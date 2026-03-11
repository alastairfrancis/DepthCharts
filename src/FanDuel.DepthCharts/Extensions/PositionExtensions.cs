namespace FanDuel.DepthCharts.Extensions;

/// <summary>
/// Extension methods for team positions.
/// </summary>
public static class PositionExtensions
{
    /// <summary>
    /// Parses a position string to the given enum type (e.g. NflPosition). Case-insensitive.
    /// </summary>
    /// <typeparam name="TPosition">Position enum type (e.g. NflPosition, MlbPosition).</typeparam>
    /// <param name="position">Position string (e.g. "QB", "LWR").</param>
    /// <returns>Position, or null if ot a valid enum value.</returns>
    public static TPosition? ToPosition<TPosition>(this string? position) where TPosition : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(position))
            return null;

        try
        {
            return Enum.Parse<TPosition>(position, ignoreCase: true);
        }
        catch (ArgumentException)
        {
            return null;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}
