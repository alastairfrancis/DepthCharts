using FanDuel.DepthCharts.Models;
using FanDuel.DepthCharts.Models.Positions;
using Microsoft.Extensions.Logging.Abstractions;

namespace FanDuel.DepthCharts.Tests;

public class DepthChartTests
{
    private readonly DepthChart<NflPosition> _chart;
    private readonly Player _tomBrady = new(12, "Tom Brady");
    private readonly Player _blaineGabbert = new(11, "Blaine Gabbert");
    private readonly Player _kyleTrask = new(2, "Kyle Trask");
    private readonly Player _mikeEvans = new(13, "Mike Evans");
    private readonly Player _jaelonDarden = new(1, "Jaelon Darden");
    private readonly Player _scottMiller = new(10, "Scott Miller");

    public DepthChartTests()
    {
        _chart = new DepthChart<NflPosition>(NullLogger<DepthChart<NflPosition>>.Instance);
        _chart.AddPlayerToDepthChart(NflPosition.QB, _tomBrady.Number, 0);
        _chart.AddPlayerToDepthChart(NflPosition.QB, _blaineGabbert.Number, 1);
        _chart.AddPlayerToDepthChart(NflPosition.QB, _kyleTrask.Number, 2);
        _chart.AddPlayerToDepthChart(NflPosition.LWR, _mikeEvans.Number, 0);
        _chart.AddPlayerToDepthChart(NflPosition.LWR, _jaelonDarden.Number, 1);
        _chart.AddPlayerToDepthChart(NflPosition.LWR, _scottMiller.Number, 2);
        _chart.AddPlayerToDepthChart(NflPosition.LWR, _kyleTrask.Number, 2);
    }

    /// <summary>getBackups: For a given player and position, returns all players that are Backups (lower position_depth).</summary>
    [Fact]
    public void GetBackups_Pos0_Returns_Lower_Order_Players()
    {
        var backups = _chart.GetBackups(NflPosition.QB, _tomBrady.Number);

        Assert.Equal(2, backups.Count);
        Assert.Equal(_blaineGabbert.Number, backups[0]);
        Assert.Equal(_kyleTrask.Number, backups[1]);
    }

    [Fact]
    public void GetBackups_Pos1_Returns_Lower_Order_Players()
    {
        var backups = _chart.GetBackups(NflPosition.LWR, _jaelonDarden.Number);

        Assert.Equal(2, backups.Count);
        Assert.Equal(_kyleTrask.Number, backups[0]);
        Assert.Equal(_scottMiller.Number, backups[1]);
    }

    [Fact]
    public void GetBackups_Player_Not_In_Position_Returns_Empty()
    {
        var backups = _chart.GetBackups(NflPosition.QB, _mikeEvans.Number);
        Assert.Empty(backups);
    }

    [Fact]
    public void GetBackups_Returns_NonEmpty()
    {
        var backups = _chart.GetBackups(NflPosition.QB, _blaineGabbert.Number);

        Assert.Single(backups);
        Assert.Equal(_kyleTrask.Number, backups[0]);
    }

    [Fact]
    public void GetBackups_Position_Last_Player_Returns_Empty()
    {
        var backups = _chart.GetBackups(NflPosition.QB, _kyleTrask.Number);
        Assert.Empty(backups);
    }

    /// <summary>getFullDepthChart: Full depth chart with every position on the team and every player within the depth chart.</summary>
    [Fact]
    public void GetFullDepthChart_Contains_Positions_InOrder()
    {
        var full = _chart.GetFullDepthChart();

        // check positions
        Assert.True(full.ContainsKey(NflPosition.QB));
        Assert.True(full.ContainsKey(NflPosition.LWR));
        Assert.False(full.ContainsKey(NflPosition.RWR));

        // check depth counts
        Assert.Equal(3, full[NflPosition.QB].Count);
        Assert.Equal(4, full[NflPosition.LWR].Count);

        // check QB order
        Assert.Equal(_tomBrady.Number, full[NflPosition.QB][0]);
        Assert.Equal(_blaineGabbert.Number, full[NflPosition.QB][1]);
        Assert.Equal(_kyleTrask.Number, full[NflPosition.QB][2]);

        // check LWR order
        Assert.Equal(_mikeEvans.Number, full[NflPosition.LWR][0]);
        Assert.Equal(_jaelonDarden.Number, full[NflPosition.LWR][1]);
        Assert.Equal(_kyleTrask.Number, full[NflPosition.LWR][2]);
        Assert.Equal(_scottMiller.Number, full[NflPosition.LWR][3]);
    }

    [Fact]
    public void RemovePlayerFromDepthChart_Returns_Player_And_Removes_From_Chart()
    {
        var removed = _chart.RemovePlayerFromDepthChart(NflPosition.LWR, _mikeEvans.Number);
        Assert.Single(removed);
        Assert.Equal(_mikeEvans.Number, removed[0]);

        var lwr = _chart.GetFullDepthChart()[NflPosition.LWR];
        Assert.Equal(3, lwr.Count);
        Assert.Equal(_jaelonDarden.Number, lwr[0]);
        Assert.Equal(_kyleTrask.Number, lwr[1]);
        Assert.Equal(_scottMiller.Number, lwr[2]);
    }

    [Fact]
    public void RemovePlayerFromDepthChart_Position_NotOnChart_Returns_Empty()
    {
        var chart = new DepthChart<NflPosition>(NullLogger<DepthChart<NflPosition>>.Instance);
        chart.AddPlayerToDepthChart(NflPosition.LWR, _mikeEvans.Number, 0);

        var removed = chart.RemovePlayerFromDepthChart(NflPosition.WR, _mikeEvans.Number);
        Assert.Empty(removed);
        Assert.Single(chart.GetFullDepthChart()[NflPosition.LWR]);
    }

    [Fact]
    public void RemovePlayerFromDepthChart_PlayerNotAtPosition_ReturnsEmpty()
    {
        var chart = new DepthChart<NflPosition>(NullLogger<DepthChart<NflPosition>>.Instance);
        chart.AddPlayerToDepthChart(NflPosition.QB, _tomBrady.Number, 0);
        chart.AddPlayerToDepthChart(NflPosition.LWR, _mikeEvans.Number, 0);

        var removed = chart.RemovePlayerFromDepthChart(NflPosition.LWR, _tomBrady.Number);
        Assert.Empty(removed);
        Assert.Single(chart.GetFullDepthChart()[NflPosition.QB]);
        Assert.Single(chart.GetFullDepthChart()[NflPosition.LWR]);
    }

    [Fact]
    public void RemovePlayerFromDepthChart_UnknownPosition_ReturnsEmpty()
    {
        var removed = _chart.RemovePlayerFromDepthChart(NflPosition.K, _tomBrady.Number);
        Assert.Empty(removed);
    }

    [Fact]
    public void AddPlayerToDepthChart_AddsPlayerAtGivenPosition()
    {
        var chart = new DepthChart<NflPosition>(NullLogger<DepthChart<NflPosition>>.Instance);
        chart.AddPlayerToDepthChart(NflPosition.QB, _tomBrady.Number, 0);

        var full = chart.GetFullDepthChart();

        Assert.True(full.ContainsKey(NflPosition.QB));
        Assert.Single(full[NflPosition.QB]);
        Assert.Equal(_tomBrady.Number, full[NflPosition.QB][0]);
    }

    [Fact]
    public void AddPlayerToDepthChart_WithoutPositionDepth_AddsAtEnd()
    {
        var chart = new DepthChart<NflPosition>(NullLogger<DepthChart<NflPosition>>.Instance);
        chart.AddPlayerToDepthChart(NflPosition.QB, _tomBrady.Number);
        chart.AddPlayerToDepthChart(NflPosition.QB, _blaineGabbert.Number);

        var full = chart.GetFullDepthChart();
        Assert.Equal(2, full[NflPosition.QB].Count);
        Assert.Equal(_tomBrady.Number, full[NflPosition.QB][0]);
        Assert.Equal(_blaineGabbert.Number, full[NflPosition.QB][1]);
    }

    [Fact]
    public void AddPlayerToDepthChart_WithPositionDepth_InsertsAndShiftsDown()
    {
        var chart = new DepthChart<NflPosition>(NullLogger<DepthChart<NflPosition>>.Instance);
        chart.AddPlayerToDepthChart(NflPosition.QB, _tomBrady.Number, 0);
        chart.AddPlayerToDepthChart(NflPosition.QB, _kyleTrask.Number, 1);
        chart.AddPlayerToDepthChart(NflPosition.QB, _blaineGabbert.Number, 1); // insert at 1, Trask moves to 2

        var full = chart.GetFullDepthChart();
        Assert.Equal(3, full[NflPosition.QB].Count);
        Assert.Equal(_tomBrady.Number, full[NflPosition.QB][0]);
        Assert.Equal(_blaineGabbert.Number, full[NflPosition.QB][1]);
        Assert.Equal(_kyleTrask.Number, full[NflPosition.QB][2]);
    }

    [Fact]
    public void AddPlayerToDepthChart_DuplicatePlayerAtSamePosition_ReturnsFalse_AndChartUnchanged()
    {
        var chart = new DepthChart<NflPosition>(NullLogger<DepthChart<NflPosition>>.Instance);
        var added = chart.AddPlayerToDepthChart(NflPosition.QB, _tomBrady.Number, 0);
        Assert.True(added);

        var duplicateAdded = chart.AddPlayerToDepthChart(NflPosition.QB, _tomBrady.Number, 0);
        Assert.False(duplicateAdded);

        var full = chart.GetFullDepthChart();
        Assert.Single(full[NflPosition.QB]);
        Assert.Equal(_tomBrady.Number, full[NflPosition.QB][0]);
    }

    [Fact]
    public void AddPlayerToDepthChart_DuplicatePlayerIdInPositionList_NotAdded_ReturnsFalse()
    {
        var chart = new DepthChart<NflPosition>(NullLogger<DepthChart<NflPosition>>.Instance);
        chart.AddPlayerToDepthChart(NflPosition.QB, _tomBrady.Number, 0);
        chart.AddPlayerToDepthChart(NflPosition.QB, _blaineGabbert.Number, 1);

        var duplicateAtDifferentDepth = chart.AddPlayerToDepthChart(NflPosition.QB, _tomBrady.Number, 1);
        Assert.False(duplicateAtDifferentDepth);

        var full = chart.GetFullDepthChart();
        Assert.Equal(2, full[NflPosition.QB].Count);
        Assert.Equal(_tomBrady.Number, full[NflPosition.QB][0]);
        Assert.Equal(_blaineGabbert.Number, full[NflPosition.QB][1]);
        Assert.Equal(1, full[NflPosition.QB].Count(id => id == _tomBrady.Number));
    }

    [Fact]
    public void SamePlayer_MultiplePositions_PlayerCanBeOnBoth()
    {
        var chart = new DepthChart<NflPosition>(NullLogger<DepthChart<NflPosition>>.Instance);
        var joshWells = new Player(72, "Josh Wells");

        chart.AddPlayerToDepthChart(NflPosition.LT, joshWells.Number, 1);
        chart.AddPlayerToDepthChart(NflPosition.RT, joshWells.Number, 1);

        var full = chart.GetFullDepthChart();
        Assert.Contains(joshWells.Number, full[NflPosition.LT].ToList());
        Assert.Contains(joshWells.Number, full[NflPosition.RT].ToList());
    }

    [Fact]
    public void GetBackups_UnknownPosition_ReturnsEmpty()
    {
        var chart = new DepthChart<NflPosition>(NullLogger<DepthChart<NflPosition>>.Instance);
        chart.AddPlayerToDepthChart(NflPosition.QB, _tomBrady.Number, 0);

        var backups = chart.GetBackups(NflPosition.K, _tomBrady.Number);
        Assert.Empty(backups);
    }
}
