using System.Data;
using Infrastructure.Persistence.Naming;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Context;

public sealed class DbContext : IDbContext
{
    private readonly ISqlConnectionFactory _connectionFactory;
    private readonly ILogger<DbContext> _logger;
    private IDbConnection? _connection;
    private IDbTransaction? _transaction;

    public DbContext(ISqlConnectionFactory connectionFactory, ILogger<DbContext> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public IDbConnection? Connection => _connection;
    public IDbTransaction? Transaction => _transaction;

    public async Task<IDbConnection> GetConnectionAsync()
    {
        if (_connection == null)
        {
            _connection = await _connectionFactory.CreateConnectionAsync();
            _logger.LogDebug("New database connection created");
        }

        return _connection;
    }

    public async Task<IDbConnection> GetOpenConnectionAsync()
    {
        var connection = await GetConnectionAsync();
        
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
            _logger.LogDebug("Database connection opened");
        }

        return connection;
    }

    public void SetTransaction(IDbTransaction transaction)
    {
        _transaction = transaction;
        _logger.LogDebug("Transaction set on DbContext");
    }

    public async Task<bool> IsHealthyAsync()
    {
        try
        {
            var connection = await GetOpenConnectionAsync();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            command.CommandType = CommandType.Text;
            
            var result = command.ExecuteScalar();
            return result != null && result.ToString() == "1";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return false;
        }
    }

    public void Dispose()
    {
        try
        {
            _transaction?.Dispose();
            _connection?.Dispose();
            _logger.LogDebug("DbContext disposed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing DbContext");
        }
    }
}
