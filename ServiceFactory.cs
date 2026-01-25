using Microsoft.Extensions.Logging;

namespace Tyc_studio.Plugin.Database;

public interface IDatabaseServiceFactory
{
    IDatabaseService CreateService(DatabaseContext context);
    IDatabaseService CreateService(string connectionString);
}

public class DatabaseServiceFactory : IDatabaseServiceFactory
{
    private readonly ILoggerFactory _loggerFactory;

    public DatabaseServiceFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public IDatabaseService CreateService(DatabaseContext context)
    {
        var logger = _loggerFactory.CreateLogger<DatabaseService>();
        return new DatabaseService(context, logger);
    }

    public IDatabaseService CreateService(string connectionString)
    {
        return CreateService(new DatabaseContext
        {
            ConnectionString = connectionString
        });
    }
}
