using FanDuel.DepthCharts.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddServices();

var app = builder.Build();
app.AddMiddleware();
app.Run();
