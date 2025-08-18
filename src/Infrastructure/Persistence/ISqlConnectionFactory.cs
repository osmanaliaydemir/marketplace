using System.Data;

namespace Infrastructure.Persistence;

public interface ISqlConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync();
    Task<IDbConnection> CreateOpenConnectionAsync();
}
