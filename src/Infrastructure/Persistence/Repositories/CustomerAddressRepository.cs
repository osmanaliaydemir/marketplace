using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.Extensions.Logging;
using Dapper;
using Infrastructure.Persistence.Naming;

namespace Infrastructure.Persistence.Repositories;

public sealed class CustomerAddressRepository : Repository<CustomerAddress>, ICustomerAddressRepository
{
    private readonly IDbContext _context;
    private readonly ILogger<CustomerAddressRepository> _logger;

    public CustomerAddressRepository(
        IDbContext context, 
        ILogger<CustomerAddressRepository> logger, 
        ITableNameResolver tableNameResolver, 
        IColumnNameResolver columnNameResolver) : base(context, logger, tableNameResolver, columnNameResolver)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<CustomerAddress>> GetAddressesByUserIdAsync(long userId)
    {
        try
        {
            const string sql = @"
                SELECT ca.*, u.Id as UserId, u.Email, u.FullName
                FROM CustomerAddresses ca
                INNER JOIN AppUsers u ON ca.CustomerId = u.Id
                WHERE ca.CustomerId = @UserId AND ca.IsActive = 1
                ORDER BY ca.IsDefault DESC, ca.CreatedAt DESC";

            using var connection = await _context.GetConnectionAsync();
            var addresses = await connection.QueryAsync<CustomerAddress>(sql, new { UserId = userId });
            
            _logger.LogInformation("Retrieved {Count} addresses for user: {UserId}", addresses.Count(), userId);
            return addresses.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting addresses for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<CustomerAddress?> GetAddressByIdAsync(long addressId, long userId)
    {
        try
        {
            const string sql = @"
                SELECT ca.*, u.Id as UserId, u.Email, u.FullName
                FROM CustomerAddresses ca
                INNER JOIN AppUsers u ON ca.CustomerId = u.Id
                WHERE ca.Id = @AddressId AND ca.CustomerId = @UserId AND ca.IsActive = 1";

            using var connection = await _context.GetConnectionAsync();
            var address = await connection.QueryFirstOrDefaultAsync<CustomerAddress>(sql, new { AddressId = addressId, UserId = userId });
            
            if (address != null)
                _logger.LogInformation("Retrieved address: {AddressId} for user: {UserId}", addressId, userId);
            else
                _logger.LogWarning("Address not found: {AddressId} for user: {UserId}", addressId, userId);
            
            return address;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting address: {AddressId} for user: {UserId}", addressId, userId);
            throw;
        }
    }

    public async Task<CustomerAddress> CreateAddressAsync(CustomerAddress address)
    {
        try
        {
            // If this is the first address or marked as default, clear other default addresses
            if (address.IsDefault)
            {
                await ClearDefaultAddressesAsync(address.CustomerId);
            }

            const string sql = @"
                INSERT INTO CustomerAddresses (
                    CustomerId, Title, RecipientName, AddressLine1, AddressLine2, 
                    City, State, PostalCode, Phone, IsDefault, IsActive, 
                    CreatedAt, ModifiedAt
                ) VALUES (
                    @CustomerId, @Title, @RecipientName, @AddressLine1, @AddressLine2,
                    @City, @State, @PostalCode, @Phone, @IsDefault, @IsActive,
                    @CreatedAt, @ModifiedAt
                );
                SELECT CAST(SCOPE_IDENTITY() as bigint)";

            using var connection = await _context.GetConnectionAsync();
            var id = await connection.QuerySingleAsync<long>(sql, new
            {
                address.CustomerId,
                address.Title,
                address.RecipientName,
                address.AddressLine1,
                address.AddressLine2,
                address.City,
                address.State,
                address.PostalCode,
                address.Phone,
                address.IsDefault,
                address.IsActive,
                address.CreatedAt,
                address.ModifiedAt
            });

            address.Id = id;
            _logger.LogInformation("Created address: {AddressId} for user: {UserId}", id, address.CustomerId);
            return address;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating address for user: {UserId}", address.CustomerId);
            throw;
        }
    }

    public async Task<CustomerAddress> UpdateAddressAsync(CustomerAddress address)
    {
        try
        {
            // If this address is being set as default, clear other default addresses
            if (address.IsDefault)
            {
                await ClearDefaultAddressesAsync(address.CustomerId);
            }

            const string sql = @"
                UPDATE CustomerAddresses SET
                    Title = @Title,
                    RecipientName = @RecipientName,
                    AddressLine1 = @AddressLine1,
                    AddressLine2 = @AddressLine2,
                    City = @City,
                    State = @State,
                    PostalCode = @PostalCode,
                    Phone = @Phone,
                    IsDefault = @IsDefault,
                    IsActive = @IsActive,
                    ModifiedAt = @ModifiedAt
                WHERE Id = @Id AND CustomerId = @CustomerId";

            using var connection = await _context.GetConnectionAsync();
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                address.Id,
                address.CustomerId,
                address.Title,
                address.RecipientName,
                address.AddressLine1,
                address.AddressLine2,
                address.City,
                address.State,
                address.PostalCode,
                address.Phone,
                address.IsDefault,
                address.IsActive,
                address.ModifiedAt
            });

            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"Address not found or access denied: {address.Id}");
            }

            _logger.LogInformation("Updated address: {AddressId} for user: {UserId}", address.Id, address.CustomerId);
            return address;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating address: {AddressId} for user: {UserId}", address.Id, address.CustomerId);
            throw;
        }
    }

    public async Task<bool> DeleteAddressAsync(long addressId, long userId)
    {
        try
        {
            // Soft delete - mark as inactive
            const string sql = @"
                UPDATE CustomerAddresses SET
                    IsActive = 0,
                    ModifiedAt = @ModifiedAt
                WHERE Id = @AddressId AND CustomerId = @UserId";

            using var connection = await _context.GetConnectionAsync();
            var rowsAffected = await connection.ExecuteAsync(sql, new { AddressId = addressId, UserId = userId, ModifiedAt = DateTime.UtcNow });
            
            if (rowsAffected > 0)
            {
                _logger.LogInformation("Deleted address: {AddressId} for user: {UserId}", addressId, userId);
                return true;
            }
            
            _logger.LogWarning("Address not found for deletion: {AddressId} for user: {UserId}", addressId, userId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting address: {AddressId} for user: {UserId}", addressId, userId);
            throw;
        }
    }

    public async Task<List<CustomerAddress>> GetDefaultAddressesByUserIdAsync(long userId)
    {
        try
        {
            const string sql = @"
                SELECT ca.*, u.Id as UserId, u.Email, u.FullName
                FROM CustomerAddresses ca
                INNER JOIN AppUsers u ON ca.CustomerId = u.Id
                WHERE ca.CustomerId = @UserId AND ca.IsDefault = 1 AND ca.IsActive = 1";

            using var connection = await _context.GetConnectionAsync();
            var addresses = await connection.QueryAsync<CustomerAddress>(sql, new { UserId = userId });
            
            _logger.LogInformation("Retrieved {Count} default addresses for user: {UserId}", addresses.Count(), userId);
            return addresses.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting default addresses for user: {UserId}", userId);
            throw;
        }
    }

    public async Task ClearDefaultAddressesAsync(long userId)
    {
        try
        {
            const string sql = @"
                UPDATE CustomerAddresses SET
                    IsDefault = 0,
                    ModifiedAt = @ModifiedAt
                WHERE CustomerId = @UserId AND IsDefault = 1 AND IsActive = 1";

            using var connection = await _context.GetConnectionAsync();
            var rowsAffected = await connection.ExecuteAsync(sql, new { UserId = userId, ModifiedAt = DateTime.UtcNow });
            
            if (rowsAffected > 0)
            {
                _logger.LogInformation("Cleared {Count} default addresses for user: {UserId}", rowsAffected, userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing default addresses for user: {UserId}", userId);
            throw;
        }
    }
}
