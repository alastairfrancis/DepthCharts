using FanDuel.DepthCharts.Repositories;
using FanDuel.DepthCharts.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FanDuel.DepthCharts.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers services for the depth chart library.
    /// </summary>
    public static IServiceCollection AddDepthChartServices(this IServiceCollection services)
    {
        // Repositories would need some configuration in practice, and it would be registered here.
        // For this exercise, we'll just use in-memory implementation.

        services
            .AddLogging()
            .AddSingleton<IPlayerRepository, InMemoryPlayerRepository>()
            .AddSingleton<IDepthChartFactory, DepthChartFactory>();

        return services;
    }
}