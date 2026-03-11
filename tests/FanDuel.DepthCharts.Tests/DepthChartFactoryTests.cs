using FanDuel.DepthCharts.Models.Positions;
using FanDuel.DepthCharts.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace FanDuel.DepthCharts.Tests;

public class DepthChartFactoryTests
{
    private static IDepthChartFactory CreateFactory()
        => new DepthChartFactory(NullLoggerFactory.Instance);

    [Fact]
    public void CreateChart_ReturnsNonNullChart()
    {
        var factory = CreateFactory();
        var chart = factory.CreateChart<NflPosition>("Tampa");
        Assert.NotNull(chart);
    }

    [Fact]
    public void CreateChart_DifferentTeamIds_ReturnsDifferentInstances()
    {
        var factory = CreateFactory();
        var chart1 = factory.CreateChart<NflPosition>("Tampa");
        var chart2 = factory.CreateChart<NflPosition>("Denver");
        Assert.NotSame(chart1, chart2);
    }

    [Fact]
    public void GetChart_WhenNoChartExists_ReturnsNull()
    {
        var factory = CreateFactory();
        var chart = factory.GetChart<NflPosition>("Tampa");

        Assert.Null(chart);
    }

    [Fact]
    public void GetChart_AfterCreateChart_ReturnsSameChart()
    {
        var factory = CreateFactory();
        var created = factory.CreateChart<NflPosition>("Tampa");
        var retrieved = factory.GetChart<NflPosition>("Tampa");

        Assert.NotNull(retrieved);
        Assert.Same(created, retrieved);
    }

    [Fact]
    public void GetOrCreateChart_WhenNoChart_CreatesAndReturnsChart()
    {
        var factory = CreateFactory();
        var chart = factory.GetOrCreateChart<NflPosition>("Tampa");
        Assert.NotNull(chart);

        chart.AddPlayerToDepthChart(NflPosition.QB, 12, 0);
        var full = chart.GetFullDepthChart();
        Assert.Single(full[NflPosition.QB]);
    }

    [Fact]
    public void GetOrCreateChart_WhenChartExists_ReturnsSameChart()
    {
        var factory = CreateFactory();
        var created = factory.CreateChart<NflPosition>("Tampa");
        created?.AddPlayerToDepthChart(NflPosition.QB, 12, 0);

        var retrieved = factory.GetOrCreateChart<NflPosition>("Tampa");
        Assert.Same(created, retrieved);

        var full = retrieved?.GetFullDepthChart();
        Assert.NotNull(full);
        Assert.Single(full[NflPosition.QB]);
        Assert.Equal(12, full[NflPosition.QB][0]);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateChart_EmptyTeamId_ThrowsArgumentException(string teamId)
    {
        var factory = CreateFactory();
        var ex = Assert.Throws<ArgumentException>(() => factory.CreateChart<NflPosition>(teamId));

        Assert.Equal("teamId", ex.ParamName);
        Assert.Contains("empty", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("\t")]
    [InlineData("         ")]
    public void GetChart_NullTeamId_ThrowsArgumentException(string teamId)
    {
        var factory = CreateFactory();
        var ex = Assert.Throws<ArgumentException>(() => factory.GetChart<NflPosition>(teamId));
        Assert.Equal("teamId", ex.ParamName);
    }

    [Theory]
    [InlineData("\t")]
    [InlineData("         ")]
    public void GetChart_EmptyTeamId_ThrowsArgumentException(string teamId)
    {
        var factory = CreateFactory();
        var ex = Assert.Throws<ArgumentException>(() => factory.GetChart<NflPosition>(teamId));
        Assert.Equal("teamId", ex.ParamName);
    }
}
