using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SqlConnectionFactory> _logger;
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration, ILogger<SqlConnectionFactory> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _connectionString = _configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("DefaultConnection connection string not found");
    }

    public async Task<IDbConnection> CreateConnectionAsync()
    {
        try
        {
            var connection = new SqlConnection(_connectionString);
            _logger.LogDebug("SQL connection created");
            return connection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating SQL connection");
            throw;
        }
    }

    public async Task<IDbConnection> CreateOpenConnectionAsync()
    {
        try
        {
            var connection = await CreateConnectionAsync();
            connection.Open();
            _logger.LogDebug("SQL connection opened");
            return connection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening SQL connection");
            throw;
        }
    }
}
