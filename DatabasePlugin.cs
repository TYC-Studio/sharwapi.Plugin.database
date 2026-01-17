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

    //di config
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDatabaseServiceFactory, DatabaseServiceFactory>();

        services.AddOptions<DatabasePluginOptions>()
            .Bind(configuration.GetSection(DatabasePluginOptions.SectionName));
            //.ValidateDataAnnotations();
        services.AddScoped<IDatabaseService>(serviceProvider =>
        {
            var contextAccessor = serviceProvider.GetService<IDatabaseContextAccessor>();
            var context = contextAccessor?.GetContext();

            if (context == null)
            {
                throw new InvalidOperationException(
                    "Database context is not set for current scope. " +
                    "Please set context using IDatabaseContextAccessor.SetContext()");
            }

            var factory = serviceProvider.GetRequiredService<IDatabaseServiceFactory>();
            return factory.CreateService(context);
        });
    }
    //middleware
    public void Configure(WebApplication app) { }
    //route config for web
    public void RegisterRoutes(IEndpointRouteBuilder app, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>($"{DatabasePluginOptions.SectionName}:ExposePluginWebApi") == true)
        {
            //TODO: plugin api expose
            //var pluggroup = app.MapGroup("/plugin").WithTags("DBPluginApi");
        }
    }
}
