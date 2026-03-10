using FanDuel.DepthCharts.Models;
using FanDuel.DepthCharts.Repositories;
using Microsoft.Extensions.Logging.Abstractions;

namespace FanDuel.DepthCharts.Tests;

public class InMemoryPlayerRepositoryTests
{
    private static InMemoryPlayerRepository CreateRepository()
        => new(NullLogger<InMemoryPlayerRepository>.Instance);

    [Fact]
    public void GetById_WhenRepositoryEmpty_ReturnsNull()
    {
        var repo = CreateRepository();
        Assert.Null(repo.GetById(12));
    }

    [Fact]
    public void GetById_WhenPlayerDoesNotExist_ReturnsNull()
    {
        var repo = CreateRepository();
        repo.Create(new Player(10, "Scott Miller"));

        Assert.Null(repo.GetById(99));
    }

    [Fact]
    public void GetById_WhenPlayerExists_ReturnsPlayer()
    {
        var repo = CreateRepository();
        var player = new Player(12, "Tom Brady");

        repo.Create(player);
        var found = repo.GetById(12);

        Assert.NotNull(found);
        Assert.Equal(12, found.Number);
        Assert.Equal("Tom Brady", found.Name);
    }

    [Fact]
    public void Create_WhenPlayerDoesNotExist_StoresPlayer()
    {
        var repo = CreateRepository();
        var player = new Player(7, "Mike Evans");

        repo.Create(player);
        var found = repo.GetById(7);

        Assert.NotNull(found);
        Assert.Equal(7, found.Number);
        Assert.Equal("Mike Evans", found.Name);
    }

    [Fact]
    public void Create_WhenPlayerAlreadyExists_ReturnsFalse()
    {
        var repo = CreateRepository();
        repo.Create(new Player(11, "Blaine Gabbert"));

        var duplicate = new Player(11, "Other Player");
        var result = repo.Create(duplicate);
        Assert.False(result);

        var found = repo.GetById(11);
        Assert.NotNull(found);
        Assert.Equal("Blaine Gabbert", found.Name);
    }

    [Fact]
    public void Update_WhenPlayerExists_UpdatesPlayer()
    {
        var repo = CreateRepository();
        repo.Create(new Player(2, "Kyle Trask"));

        var updated = new Player(2, "Kyle Trask Jr.");
        repo.Update(updated);
        
        var found = repo.GetById(2);
        Assert.NotNull(found);
        Assert.Equal("Kyle Trask Jr.", found.Name);
    }

    [Fact]
    public void Update_WhenPlayerDoesNotExist_ReturnsFalse()
    {
        var repo = CreateRepository();
        var player = new Player(99, "Unknown");
        var result = repo.Update(player);

        Assert.False(result);
    }

    [Fact]
    public void Create_MultiplePlayers_AllRetrievableById()
    {
        var repo = CreateRepository();
        var players = new[]
        {
            new Player(1, "Jaelon Darden"),
            new Player(2, "Kyle Trask"),
            new Player(12, "Tom Brady"),
        };

        foreach (var p in players)
        {
            repo.Create(p);
        }

        foreach (var p in players)
        {
            var player = repo.GetById(p.Number);

            Assert.NotNull(player);
            Assert.Equal(p.Number, player.Number);
            Assert.Equal(p.Name, player.Name);
        }
    }
}
