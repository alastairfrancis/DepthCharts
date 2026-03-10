using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace FanDuel.DepthCharts.Services;

/// <summary>
/// Factory that creates or returns a depth chart per teamId. 
/// Depth charts are created as singletons because they are stored in-memory.
/// 
/// In a practical system, the charts are likely to be stored in a distributed cache or database, and the factory 
/// would manage access to that storage instead of keeping charts in memory. 
/// 
/// Added GetOrCreateChart() method to simplify usage as the data is stored in-memory.  
/// Normally the Get / Create pattern is enough, but this method provides a convenient way to do that in one call.
/// </summary>
public sealed class DepthChartFactory(ILoggerFactory loggerFactory) : IDepthChartFactory
{
    private readonly ILoggerFactory _loggerFactory = loggerFactory;
    private readonly ILogger<DepthChartFactory> _logger = loggerFactory.CreateLogger<DepthChartFactory>();
    private readonly ConcurrentDictionary<string, object> _charts = new();

    public IDepthChart<TPosition>? CreateChart<TPosition>(string teamId) where TPosition : struct, Enum
    {
        ValidateTeamId(teamId);
        var chart = CreateTypedChart<TPosition>();
        
        if (_charts.TryAdd(teamId, chart))
        {
            _logger.LogInformation("Created depth chart for TeamId={TeamId}", teamId);
            return chart;
        }

        return null;
    }

    public IDepthChart<TPosition>? GetChart<TPosition>(string teamId) where TPosition : struct, Enum
    {
        ValidateTeamId(teamId);
        if (_charts.TryGetValue(teamId, out var entry))
        {
            return (IDepthChart<TPosition>)entry;
        }

        return null;
    }

    public IDepthChart<TPosition>? GetOrCreateChart<TPosition>(string teamId) where TPosition : struct, Enum
    {
        ValidateTeamId(teamId);
        var chart = CreateTypedChart<TPosition>();

        if (_charts.TryAdd(teamId, chart))
        {
            _logger.LogInformation("Created depth chart for TeamId={TeamId}", teamId);
            return chart;
        }
        else 
        {
            if (_charts.TryGetValue(teamId, out var entry))
            {
                return (IDepthChart<TPosition>)entry;
            }
        }

        return null;
    }

    private DepthChart<TPosition> CreateTypedChart<TPosition>() where TPosition : struct, Enum
    {
        var chartLogger = _loggerFactory.CreateLogger<DepthChart<TPosition>>();
        return new DepthChart<TPosition>(chartLogger);
    }

    private static void ValidateTeamId(string? teamId)
    {
        ArgumentNullException.ThrowIfNull(teamId);
        if (string.IsNullOrWhiteSpace(teamId))
            throw new ArgumentException("TeamId cannot be empty or whitespace.", nameof(teamId));
    }
}
