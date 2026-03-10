namespace FanDuel.DepthCharts.Models;

/// <summary>
/// Represents a player. The number uniquely identifies the player within a team.
/// A player can appear on the depth chart at multiple positions.
/// </summary>
public record Player(int Number, string Name)
{
    public override string ToString() => $"#{Number} – {Name}";
}
