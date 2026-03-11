using FanDuel.DepthCharts.Models;

namespace FanDuel.DepthCharts.Repositories;

/// <summary>
/// Provides player information.
/// </summary>
public interface IPlayerRepository
{
    /// <summary>
    /// Gets a player by id (player number).
    /// </summary>
    /// <returns>Player, or null if not found.</returns>
    Player? GetById(long id);

    /// <summary>
    /// Adds a new player. Id is the player number.
    /// </summary>
    /// <returns>true on success, false if player exists, or creation fails.</returns>
    bool Create(Player player);

    /// <summary>
    /// Updates an existing player with same player number.
    /// </summary>
    /// <returns>true on success, false if player does not exist, or update fails.</returns>
    bool Update(Player player);
}
