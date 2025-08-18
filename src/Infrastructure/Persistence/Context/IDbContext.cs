using System.Data;

namespace Infrastructure.Persistence.Context;

public interface IDbContext : IDisposable
{
    IDbConnection? Connection { get; }
    IDbTransaction? Transaction { get; }
    
    Task<IDbConnection> GetConnectionAsync();
    Task<IDbConnection> GetOpenConnectionAsync();
    void SetTransaction(IDbTransaction transaction);
    
    Task<bool> IsHealthyAsync();
}
