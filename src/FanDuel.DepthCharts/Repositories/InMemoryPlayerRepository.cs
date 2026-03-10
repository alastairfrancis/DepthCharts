using FanDuel.DepthCharts.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace FanDuel.DepthCharts.Repositories;

/// <summary>
/// In-memory player repository.
/// In practice, this would be implemented with a database, or fetched from an external service.
/// </summary>
public class InMemoryPlayerRepository(
        ILogger<InMemoryPlayerRepository> logger
    ) : IPlayerRepository
{
    private readonly ILogger<InMemoryPlayerRepository> _logger = logger;
    private readonly ConcurrentDictionary<long, Player> _players = new();

    public Player? GetById(long id)
    {
        return _players.TryGetValue(id, out var p) ? p : null;
    }

    public bool Create(Player player)
    {
        if (!_players.TryAdd(player.Number, player))
        {
            _logger.LogWarning("Create failed: player already exists. PlayerId={PlayerId}, PlayerName={PlayerName}", player.Number, player.Name);
            return false;
        }

        _logger.LogInformation("Player created. PlayerId={PlayerId}, PlayerName={PlayerName}", player.Number, player.Name);
        return true;
    }

    public bool Update(Player player)
    {
        try
        {
            _players.TryUpdate(player.Number, player, _players[player.Number]);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Update failed: player not found. PlayerId={PlayerId}", player.Number);
            return false;
        }

        _logger.LogInformation("Player updated. PlayerId={PlayerId}, PlayerName={PlayerName}", player.Number, player.Name);
        return true;
    }
}
