using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FanDuel.DepthCharts.Api.Swagger;

/// <summary>
/// Removes the root GET / redirect from the Swagger document so only controller APIs are listed.
/// </summary>
public class RemoveRootEndpointDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Paths.Remove("/");
    }
}
