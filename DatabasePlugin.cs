using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using sharwapi.Contracts.Core;

namespace sharwapi.Plugin.database;

public class DatabasePlugin : IApiPlugin
{
    public string Name => "database";
    public string DisplayName => "Database";
    public string Version => "0.1.0";
    
    public void Configure(WebApplication app) { }
    public void RegisterServices(IServiceCollection services, IConfiguration configuration) { }

    public void RegisterRoutes(IEndpointRouteBuilder app, IConfiguration configuration)
    {
        var group = app.MapGroup("/database");
    }
}