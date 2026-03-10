using FanDuel.DepthCharts.Api.Swagger;
using FanDuel.DepthCharts.Extensions;

namespace FanDuel.DepthCharts.Api.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers services for the depth chart library.
    /// </summary>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddDepthChartServices();
        services.AddControllers();

        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "FanDuel Depth Charts API",
                        Version = "v1",
                        Description = "APIs to create depth charts and add players by sport and team."
                    });
                    options.DocumentFilter<RemoveRootEndpointDocumentFilter>();
                });

        return services;
    }

    /// <summary>
    /// Adds middleware to the application.
    /// </summary>
    public static WebApplication AddMiddleware(this WebApplication app)
    {
        app
            .UseSwagger()
            .UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "FanDuel Depth Charts API v1");
            });

        app.MapGet("/", () => Results.Redirect("/swagger"));
        app.MapControllers();

        return app;
    }
}