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
        /*services.AddSingleton<IDatabaseContext>(serviceProvider =>
        {
            return new DatabaseContext
            {
                ConnectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentException("DefaultConnection is required to be configured."),
                DatabaseType = configuration.GetValue<string>($"{DatabaseContext.SectionName}:DatabaseType") ?? throw new ArgumentException("DatabaseType is required to be configured."),
                CommandTimeout = configuration.GetValue<int>($"{DatabaseContext.SectionName}:CommandTimeout"),
                UseTransaction = configuration.GetValue<bool>($"{DatabaseContext.SectionName}:UseTransaction"),
            };
        });   仅供演示，用户需像这样注册一个IDatabaseContext */
        
        services.AddSingleton<IDatabaseServiceFactory, DatabaseServiceFactory>();

        services.AddOptions<DatabasePluginOptions>()
            .Bind(configuration.GetSection(DatabasePluginOptions.SectionName));
            //.ValidateDataAnnotations();
        
        services.AddScoped<IDatabaseService>(serviceProvider =>
        {
            var context = serviceProvider.GetService<DatabaseContext>() ?? throw new ArgumentException("DatabaseContext is required to be registered.");
            var factory = serviceProvider.GetRequiredService<IDatabaseServiceFactory>();
            
            return factory.CreateService(context);
        });
    }
    
    //middleware
    public void Configure(WebApplication app) { }
    
    //route config for web
    public void RegisterRoutes(IEndpointRouteBuilder app, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>($"{DatabasePluginOptions.SectionName}:ExposePluginWebApi"))
        {
            //TODO: plugin api expose
            var group = app.MapGroup($"/{Name}");
            group.MapGet("/test", async (IDatabaseService dbService) =>
            {
                return await dbService.TestConnectionAsync();
            });
        }
    }
}
