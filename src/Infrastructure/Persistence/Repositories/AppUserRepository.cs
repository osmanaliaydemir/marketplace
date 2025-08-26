using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Naming;
using Microsoft.Extensions.Logging;
using Dapper;

namespace Infrastructure.Persistence.Repositories;

public sealed class AppUserRepository : AuditableRepository<AppUser>, IAppUserRepository
{
    public AppUserRepository(
        IDbContext context, 
        ILogger<AppUserRepository> logger,
        ITableNameResolver tableNameResolver,
        IColumnNameResolver columnNameResolver) 
        : base(context, logger, tableNameResolver, columnNameResolver)
    {
    }

    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        var sql = "SELECT * FROM app_users WHERE email = @Email AND is_deleted = 0";
        var connection = await _dbContext.GetConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<AppUser>(sql, new { Email = email });
    }

    public async Task<AppUser?> GetByUsernameAsync(string username)
    {
        var sql = "SELECT * FROM app_users WHERE full_name = @Username AND is_deleted = 0";
        var connection = await _dbContext.GetConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<AppUser>(sql, new { Username = username });
    }

    public async Task<AppUser?> GetByEmailOrUsernameAsync(string emailOrUsername)
    {
        var sql = "SELECT * FROM app_users WHERE (email = @EmailOrUsername OR full_name = @EmailOrUsername) AND is_deleted = 0";
        var connection = await _dbContext.GetConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<AppUser>(sql, new { EmailOrUsername = emailOrUsername });
    }

    public async Task<bool> IsEmailUniqueAsync(string email, long? excludeUserId = null)
    {
        string sql;
        object parameters;
        
        if (excludeUserId.HasValue)
        {
            sql = "SELECT COUNT(*) FROM app_users WHERE email = @Email AND id != @ExcludeUserId AND is_deleted = 0";
            parameters = new { Email = email, ExcludeUserId = excludeUserId.Value };
        }
        else
        {
            sql = "SELECT COUNT(*) FROM app_users WHERE email = @Email AND is_deleted = 0";
            parameters = new { Email = email };
        }
        
        var connection = await _dbContext.GetConnectionAsync();
        var count = await connection.QuerySingleAsync<int>(sql, parameters);
        return count == 0;
    }

    public async Task<bool> IsUsernameUniqueAsync(string username, long? excludeUserId = null)
    {
        string sql;
        object parameters;
        
        if (excludeUserId.HasValue)
        {
            sql = "SELECT COUNT(*) FROM app_users WHERE full_name = @Username AND id != @ExcludeUserId AND is_deleted = 0";
            parameters = new { Username = username, ExcludeUserId = excludeUserId.Value };
        }
        else
        {
            sql = "SELECT COUNT(*) FROM app_users WHERE full_name = @Username AND is_deleted = 0";
            parameters = new { Username = username };
        }
        
        var connection = await _dbContext.GetConnectionAsync();
        var count = await connection.QuerySingleAsync<int>(sql, parameters);
        return count == 0;
    }
}
