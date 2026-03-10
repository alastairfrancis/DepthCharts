using FanDuel.DepthCharts.Helpers;
using FanDuel.DepthCharts.Models;
using FanDuel.DepthCharts.Models.Positions;

namespace FanDuel.DepthCharts.Tests;

public class SportPositionTypeTests
{
    [Fact]
    public void GetPositionType_NFL_ReturnsNflPosition()
    {
        var type = SportPositionType.GetPositionType(Sport.NFL);
        Assert.NotNull(type);
        Assert.Equal(typeof(NflPosition), type);
    }

    [Fact]
    public void GetPositionType_MLB_ReturnsMlbPosition()
    {
        var type = SportPositionType.GetPositionType(Sport.MLB);
        Assert.NotNull(type);
        Assert.Equal(typeof(MlbPosition), type);
    }

    [Fact]
    public void GetPositionType_NHL_ReturnsNhlPosition()
    {
        var type = SportPositionType.GetPositionType(Sport.NHL);
        Assert.NotNull(type);
        Assert.Equal(typeof(NhlPosition), type);
    }

    [Fact]
    public void GetPositionType_NBA_ReturnsNbaPosition()
    {
        var type = SportPositionType.GetPositionType(Sport.NBA);
        Assert.NotNull(type);
        Assert.Equal(typeof(NbaPosition), type);
    }

    [Fact]
    public void GetPositionType_UnregisteredSport_ReturnsNull()
    {
        // Use a value not in the default map (e.g. cast an out-of-range int)
        var unknownSport = (Sport)999;
        var type = SportPositionType.GetPositionType(unknownSport);
        Assert.Null(type);
    }
}
