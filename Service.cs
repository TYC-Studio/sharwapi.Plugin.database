using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace sharwapi.Plugin.database;

public interface IDatabaseService
{
    DatabaseContext GetContext();
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null);
    Task<int> ExecuteAsync(string sql, object? parameters = null);
    Task<bool> TestConnectionAsync();
}

public class DatabaseService : IDatabaseService, IDisposable
{
    private readonly DatabaseContext _context;
    private readonly SqlConnection _connection;
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseContext GetContext() => _context;

    public DatabaseService(
        DatabaseContext context,
        ILogger<DatabaseService> logger)
    {
        _context = context;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(_context.ConnectionString))
        {
            throw new ArgumentException("Connection string is required");
        }

        _connection = new SqlConnection(_context.ConnectionString);
        _logger.LogDebug("DatabaseService created.");
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            await using var connection = new SqlConnection(_context.ConnectionString);
            await connection.OpenAsync();
            await connection.CloseAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection test failed");
            return false;
        }
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters)
    {
        if (string.IsNullOrWhiteSpace(sql))
        {
            _logger.LogWarning("QueryAsync called with empty SQL.");
            return [];
        }

        try
        {
            await using var connection = new SqlConnection(_context.ConnectionString);
            await connection.OpenAsync();
            var result = await connection.QueryAsync<T>(sql, parameters);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "QueryAsync failed. SQL: {Sql}", sql);
            return [];
        }
    }

    public async Task<int> ExecuteAsync(string sql, object? parameters)
    {
        if (string.IsNullOrWhiteSpace(sql))
        {
            _logger.LogWarning("ExecuteAsync called with empty SQL.");
            return -1;
        }

        try
        {
            await using var connection = new SqlConnection(_context.ConnectionString);
            await connection.OpenAsync();
            var affected = await connection.ExecuteAsync(sql, parameters);
            return affected;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ExecuteAsync failed. SQL: {Sql}", sql);
            return -1;
        }
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
