using FanDuel.DepthCharts.Models;

namespace FanDuel.DepthCharts.Repositories;

/// <summary>
/// Provides full player information by player id (jersey number within a team).
/// </summary>
public interface IPlayerRepository
{
    /// <summary>Gets a player by id (player number), or null if not found.</summary>
    Player? GetById(long id);

    /// <summary>Adds a new player. Id is the player number.</summary>
    bool Create(Player player);

    /// <summary>Updates an existing player.</summary>
    bool Update(Player player);
}
