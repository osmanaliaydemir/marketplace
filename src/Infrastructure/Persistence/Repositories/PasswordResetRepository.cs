using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.Extensions.Logging;
using Dapper;
using Infrastructure.Persistence.Naming;

namespace Infrastructure.Persistence.Repositories;

public sealed class PasswordResetRepository : Repository<PasswordReset>, IPasswordResetRepository
{
    private readonly IDbContext _context;
    private readonly ILogger<PasswordResetRepository> _logger;

    public PasswordResetRepository(
        IDbContext context,
        ILogger<PasswordResetRepository> logger,
        ITableNameResolver tableNameResolver,
        IColumnNameResolver columnNameResolver) : base(context, logger, tableNameResolver, columnNameResolver)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PasswordReset?> GetByTokenAsync(string token)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PasswordResets 
                WHERE Token = @Token AND IsUsed = 0 AND ExpiresAt > @CurrentTime";

            using var connection = await _context.GetConnectionAsync();
            var passwordReset = await connection.QueryFirstOrDefaultAsync<PasswordReset>(sql, new { Token = token, CurrentTime = DateTime.UtcNow });
            
            if (passwordReset != null)
                _logger.LogInformation("Retrieved password reset by token: {Token}", token);
            else
                _logger.LogWarning("Password reset not found or expired for token: {Token}", token);
            
            return passwordReset;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting password reset by token: {Token}", token);
            throw;
        }
    }

    public async Task<PasswordReset?> GetByEmailAsync(string email)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PasswordResets 
                WHERE Email = @Email 
                ORDER BY CreatedAt DESC";

            using var connection = await _context.GetConnectionAsync();
            var passwordReset = await connection.QueryFirstOrDefaultAsync<PasswordReset>(sql, new { Email = email });
            
            if (passwordReset != null)
                _logger.LogInformation("Retrieved password reset by email: {Email}", email);
            else
                _logger.LogWarning("Password reset not found for email: {Email}", email);
            
            return passwordReset;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting password reset by email: {Email}", email);
            throw;
        }
    }

    public async Task<PasswordReset> CreateAsync(PasswordReset passwordReset)
    {
        try
        {
            const string sql = @"
                INSERT INTO PasswordResets (
                    Email, Token, ExpiresAt, IsUsed, UsedAt, IpAddress, UserAgent, 
                    CreatedAt, ModifiedAt
                ) VALUES (
                    @Email, @Token, @ExpiresAt, @IsUsed, @UsedAt, @IpAddress, @UserAgent,
                    @CreatedAt, @ModifiedAt
                );
                SELECT CAST(SCOPE_IDENTITY() as bigint)";

            using var connection = await _context.GetConnectionAsync();
            var id = await connection.QuerySingleAsync<long>(sql, new
            {
                passwordReset.Email,
                passwordReset.Token,
                passwordReset.ExpiresAt,
                passwordReset.IsUsed,
                passwordReset.UsedAt,
                passwordReset.IpAddress,
                passwordReset.UserAgent,
                passwordReset.CreatedAt,
                passwordReset.ModifiedAt
            });

            passwordReset.Id = id;
            _logger.LogInformation("Created password reset: {Id} for email: {Email}", id, passwordReset.Email);
            return passwordReset;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating password reset for email: {Email}", passwordReset.Email);
            throw;
        }
    }

    public async Task<PasswordReset> UpdateAsync(PasswordReset passwordReset)
    {
        try
        {
            const string sql = @"
                UPDATE PasswordResets SET
                    Email = @Email,
                    Token = @Token,
                    ExpiresAt = @ExpiresAt,
                    IsUsed = @IsUsed,
                    UsedAt = @UsedAt,
                    IpAddress = @IpAddress,
                    UserAgent = @UserAgent,
                    ModifiedAt = @ModifiedAt
                WHERE Id = @Id";

            using var connection = await _context.GetConnectionAsync();
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                passwordReset.Id,
                passwordReset.Email,
                passwordReset.Token,
                passwordReset.ExpiresAt,
                passwordReset.IsUsed,
                passwordReset.UsedAt,
                passwordReset.IpAddress,
                passwordReset.UserAgent,
                passwordReset.ModifiedAt
            });

            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"Password reset not found: {passwordReset.Id}");
            }

            _logger.LogInformation("Updated password reset: {Id} for email: {Email}", passwordReset.Id, passwordReset.Email);
            return passwordReset;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating password reset: {Id} for email: {Email}", passwordReset.Id, passwordReset.Email);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long id)
    {
        try
        {
            const string sql = @"
                DELETE FROM PasswordResets 
                WHERE Id = @Id";

            using var connection = await _context.GetConnectionAsync();
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            
            if (rowsAffected > 0)
            {
                _logger.LogInformation("Deleted password reset: {Id}", id);
                return true;
            }
            
            _logger.LogWarning("Password reset not found for deletion: {Id}", id);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting password reset: {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteExpiredAsync()
    {
        try
        {
            const string sql = @"
                DELETE FROM PasswordResets 
                WHERE ExpiresAt < @CurrentTime";

            using var connection = await _context.GetConnectionAsync();
            var rowsAffected = await connection.ExecuteAsync(sql, new { CurrentTime = DateTime.UtcNow });
            
            if (rowsAffected > 0)
            {
                _logger.LogInformation("Deleted {Count} expired password resets", rowsAffected);
            }
            
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting expired password resets");
            throw;
        }
    }

    public async Task<List<PasswordReset>> GetByEmailAndNotUsedAsync(string email)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PasswordResets 
                WHERE Email = @Email AND IsUsed = 0 AND ExpiresAt > @CurrentTime
                ORDER BY CreatedAt DESC";

            using var connection = await _context.GetConnectionAsync();
            var passwordResets = await connection.QueryAsync<PasswordReset>(sql, new { Email = email, CurrentTime = DateTime.UtcNow });
            
            _logger.LogInformation("Retrieved {Count} active password resets for email: {Email}", passwordResets.Count(), email);
            return passwordResets.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active password resets for email: {Email}", email);
            throw;
        }
    }
}
