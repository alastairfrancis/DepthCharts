using FanDuel.DepthCharts.Api.Helpers;
using FanDuel.DepthCharts.Api.Models;
using FanDuel.DepthCharts.Helpers;
using FanDuel.DepthCharts.Models;
using FanDuel.DepthCharts.Services;
using Microsoft.AspNetCore.Mvc;

namespace FanDuel.DepthCharts.Api.Endpoints;

/// <summary>Endpoints for creating and managing depth charts.</summary>
[ApiController]
[Route("api/sports/{sport}/teams/{teamId}")]
[Produces("application/json")]
[Tags("Charts")]
public class ChartsController(IDepthChartFactory factory) : ControllerBase
{
    /// <summary>Create or get a depth chart for the given sport and team.</summary>
    [HttpPost("chart")]
    [ProducesResponseType(typeof(CreateChartResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult CreateChart(Sport sport, string teamId)
    {
        var positionType = SportPositionType.GetPositionType(sport);
        if (positionType == null)
            return BadRequest($"Unsupported sport {sport}.");

        GetOrCreateChart(factory, teamId, positionType);
        return Created(
            $"/api/sports/{sport}/teams/{teamId}/chart",
            new CreateChartResponse(sport.ToString(), teamId));
    }

    /// <summary>Add a player to the depth chart at a position.</summary>
    [HttpPost("players")]
    [ProducesResponseType(typeof(AddPlayerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult AddPlayer(Sport sport, string teamId, [FromBody] AddPlayerRequest request)
    {
        var positionType = SportPositionType.GetPositionType(sport);
        if (positionType == null)
            return BadRequest($"Unsupported sport {sport}.");

        var chart = GetOrCreateChart(factory, teamId, positionType);
        ChartReflectionHelper.AddPlayerToDepthChart(chart, request.Position, request.PlayerId, request.PositionDepth);
        return Created(
            $"/api/sports/{sport}/teams/{teamId}/chart",
            new AddPlayerResponse(sport.ToString(), teamId, request.Position, request.PlayerId, request.PositionDepth));
    }

    /// <summary>Get the full depth chart (all positions and ordered player IDs).</summary>
    [HttpGet("chart")]
    [ProducesResponseType(typeof(GetFullDepthChartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetFullDepthChart(Sport sport, string teamId)
    {
        var positionType = SportPositionType.GetPositionType(sport);
        if (positionType == null)
            return BadRequest($"Unsupported sport {sport}.");

        var chart = GetOrCreateChart(factory, teamId, positionType);
        var positions = ChartReflectionHelper.GetFullDepthChartStringKeyed(chart);
        return Ok(new GetFullDepthChartResponse(positions));
    }

    /// <summary>Get backup player IDs for a given position and player.</summary>
    [HttpGet("backups")]
    [ProducesResponseType(typeof(GetBackupsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetBackups(Sport sport, string teamId, [FromQuery] string position, [FromQuery] long playerId)
    {
        var positionType = SportPositionType.GetPositionType(sport);
        if (positionType == null)
            return BadRequest($"Unsupported sport {sport}.");

        var chart = GetOrCreateChart(factory, teamId, positionType);
        var playerIds = ChartReflectionHelper.GetBackups(chart, position, playerId);
        return Ok(new GetBackupsResponse(playerIds));
    }

    /// <summary>Calls factory.GetOrCreateChart&lt;TPosition&gt;(teamId) with the runtime position type.</summary>
    private static object GetOrCreateChart(IDepthChartFactory factory, string teamId, Type positionType)
    {
        var method = typeof(IDepthChartFactory).GetMethods()
            .First(m => m.Name == nameof(IDepthChartFactory.GetOrCreateChart)
                && m.IsGenericMethodDefinition
                && m.GetParameters().Length == 1
                && m.GetParameters()[0].ParameterType == typeof(string));
        var genericMethod = method.MakeGenericMethod(positionType);
        return genericMethod.Invoke(factory, [teamId])!;
    }
}
