using FanDuel.DepthCharts.Models;
using FanDuel.DepthCharts.Repositories;
using Microsoft.Extensions.Logging.Abstractions;

namespace FanDuel.DepthCharts.Tests;

public class InMemoryPlayerRepositoryTests
{
    private const int TomBradyId = 12;
    private const int ScottMillerId = 10;
    private const int MikeEvansId = 7;
    private const int BlaineGabbertId = 11;
    private const int KyleTraskId = 2;
    private const int JaelonDardenId = 1;
    private const int NonexistentPlayerId = 99;

    private static InMemoryPlayerRepository CreateRepository()
        => new(NullLogger<InMemoryPlayerRepository>.Instance);

    [Fact]
    public void GetById_WhenRepositoryEmpty_ReturnsNull()
    {
        var repo = CreateRepository();
        Assert.Null(repo.GetById(TomBradyId));
    }

    [Fact]
    public void GetById_WhenPlayerDoesNotExist_ReturnsNull()
    {
        var repo = CreateRepository();
        repo.Create(new Player(ScottMillerId, "Scott Miller"));

        Assert.Null(repo.GetById(NonexistentPlayerId));
    }

    [Fact]
    public void GetById_WhenPlayerExists_ReturnsPlayer()
    {
        var repo = CreateRepository();
        var player = new Player(TomBradyId, "Tom Brady");

        repo.Create(player);
        var found = repo.GetById(TomBradyId);

        Assert.NotNull(found);
        Assert.Equal(TomBradyId, found.Number);
        Assert.Equal("Tom Brady", found.Name);
    }

    [Fact]
    public void Create_WhenPlayerDoesNotExist_StoresPlayer()
    {
        var repo = CreateRepository();
        var player = new Player(MikeEvansId, "Mike Evans");

        repo.Create(player);
        var found = repo.GetById(MikeEvansId);

        Assert.NotNull(found);
        Assert.Equal(MikeEvansId, found.Number);
        Assert.Equal("Mike Evans", found.Name);
    }

    [Fact]
    public void Create_WhenPlayerAlreadyExists_ReturnsFalse()
    {
        var repo = CreateRepository();
        repo.Create(new Player(BlaineGabbertId, "Blaine Gabbert"));

        var duplicate = new Player(BlaineGabbertId, "Other Player");
        var result = repo.Create(duplicate);
        Assert.False(result);

        var found = repo.GetById(BlaineGabbertId);
        Assert.NotNull(found);
        Assert.Equal("Blaine Gabbert", found.Name);
    }

    [Fact]
    public void Update_WhenPlayerExists_UpdatesPlayer()
    {
        var repo = CreateRepository();
        repo.Create(new Player(KyleTraskId, "Kyle Trask"));

        var updated = new Player(KyleTraskId, "Kyle Trask Jr.");
        repo.Update(updated);

        var found = repo.GetById(KyleTraskId);
        Assert.NotNull(found);
        Assert.Equal("Kyle Trask Jr.", found.Name);
    }

    [Fact]
    public void Update_WhenPlayerDoesNotExist_ReturnsFalse()
    {
        var repo = CreateRepository();
        var player = new Player(NonexistentPlayerId, "Unknown");
        var result = repo.Update(player);

        Assert.False(result);
    }

    [Fact]
    public void Create_MultiplePlayers_AllRetrievableById()
    {
        var repo = CreateRepository();
        var players = new[]
        {
            new Player(JaelonDardenId, "Jaelon Darden"),
            new Player(KyleTraskId, "Kyle Trask"),
            new Player(TomBradyId, "Tom Brady"),
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
