using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.Extensions.Logging;
using Dapper;
using Infrastructure.Persistence.Naming;

namespace Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    private readonly IDbContext _context;
    private readonly ILogger<CustomerRepository> _logger;

    public CustomerRepository(IDbContext context, ILogger<CustomerRepository> logger, ITableNameResolver tableNameResolver, IColumnNameResolver columnNameResolver) : base(context, logger, tableNameResolver, columnNameResolver)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Customers 
                WHERE Email = @Email";

            using var connection = await _context.GetConnectionAsync();
            var customer = await connection.QueryFirstOrDefaultAsync<Customer>(sql, new { Email = email });
            
            _logger.LogInformation("Customer retrieved by email: {Email}", email);
            return customer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer by email: {Email}", email);
            throw;
        }
    }

    public async Task<Customer?> GetByPhoneAsync(string phone)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Customers 
                WHERE Phone = @Phone";

            using var connection = await _context.GetConnectionAsync();
            var customer = await connection.QueryFirstOrDefaultAsync<Customer>(sql, new { Phone = phone });
            
            _logger.LogInformation("Customer retrieved by phone: {Phone}", phone);
            return customer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer by phone: {Phone}", phone);
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
    {
        try
        {
            const string sql = @"
                SELECT * FROM Customers 
                WHERE IsActive = 1 
                ORDER BY CreatedAt DESC";

            using var connection = await _context.GetConnectionAsync();
            var customers = await connection.QueryAsync<Customer>(sql);
            
            _logger.LogInformation("Retrieved {Count} active customers", customers.Count());
            return customers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active customers");
            throw;
        }
    }

    public async Task<bool> IsEmailUniqueAsync(string email, long? excludeCustomerId = null)
    {
        try
        {
            string sql;
            object parameters;

            if (excludeCustomerId.HasValue)
            {
                sql = @"
                    SELECT COUNT(*) FROM Customers 
                    WHERE Email = @Email AND Id != @ExcludeCustomerId";
                parameters = new { Email = email, ExcludeCustomerId = excludeCustomerId.Value };
            }
            else
            {
                sql = "SELECT COUNT(*) FROM Customers WHERE Email = @Email";
                parameters = new { Email = email };
            }

            using var connection = await _context.GetConnectionAsync();
            var count = await connection.ExecuteScalarAsync<int>(sql, parameters);
            var isUnique = count == 0;
            
            _logger.LogInformation("Email {Email} uniqueness check: {IsUnique}", email, isUnique);
            return isUnique;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email uniqueness: {Email}", email);
            throw;
        }
    }

    public async Task<bool> IsPhoneUniqueAsync(string phone, long? excludeCustomerId = null)
    {
        try
        {
            string sql;
            object parameters;

            if (excludeCustomerId.HasValue)
            {
                sql = @"
                    SELECT COUNT(*) FROM Customers 
                    WHERE Phone = @Phone AND Id != @ExcludeCustomerId";
                parameters = new { Phone = phone, ExcludeCustomerId = excludeCustomerId.Value };
            }
            else
            {
                sql = "SELECT COUNT(*) FROM Customers WHERE Phone = @Phone";
                parameters = new { Phone = phone };
            }

            using var connection = await _context.GetConnectionAsync();
            var count = await connection.ExecuteScalarAsync<int>(sql, parameters);
            var isUnique = count == 0;
            
            _logger.LogInformation("Phone {Phone} uniqueness check: {IsUnique}", phone, isUnique);
            return isUnique;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking phone uniqueness: {Phone}", phone);
            throw;
        }
    }

    public async Task<int> GetTotalCustomerCountAsync()
    {
        try
        {
            const string sql = "SELECT COUNT(*) FROM Customers";

            using var connection = await _context.GetConnectionAsync();
            var count = await connection.ExecuteScalarAsync<int>(sql);
            
            _logger.LogInformation("Total customer count: {Count}", count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total customer count");
            throw;
        }
    }

    public async Task<decimal> GetTotalCustomerSpendingAsync(long customerId)
    {
        try
        {
            const string sql = @"
                SELECT COALESCE(SUM(o.TotalAmount), 0) 
                FROM Orders o
                INNER JOIN Customers c ON o.CustomerId = c.Id
                WHERE c.Id = @CustomerId AND o.Status = 'Completed'";

            using var connection = await _context.GetConnectionAsync();
            var totalSpending = await connection.ExecuteScalarAsync<decimal>(sql, new { CustomerId = customerId });
            
            _logger.LogInformation("Total spending for customer {CustomerId}: {TotalSpending}", customerId, totalSpending);
            return totalSpending;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total spending for customer: {CustomerId}", customerId);
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> GetCustomersByDateRangeAsync(DateTime from, DateTime to)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Customers 
                WHERE CreatedAt >= @From AND CreatedAt <= @To 
                ORDER BY CreatedAt DESC";

            using var connection = await _context.GetConnectionAsync();
            var customers = await connection.QueryAsync<Customer>(sql, new { From = from, To = to });
            
            _logger.LogInformation("Retrieved {Count} customers from {From} to {To}", customers.Count(), from, to);
            return customers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customers by date range: {From} to {To}", from, to);
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> GetTopSpendingCustomersAsync(int count = 10)
    {
        try
        {
            const string sql = @"
                SELECT TOP(@Count) c.*, COALESCE(SUM(o.TotalAmount), 0) as TotalSpending
                FROM Customers c
                LEFT JOIN Orders o ON c.Id = o.CustomerId AND o.Status = 'Completed'
                GROUP BY c.Id, c.FirstName, c.LastName, c.Email, c.Phone, c.DateOfBirth, c.Gender, c.IsActive, c.IsVerified, c.CreatedAt, c.ModifiedAt
                ORDER BY TotalSpending DESC";

            using var connection = await _context.GetConnectionAsync();
            var customers = await connection.QueryAsync<Customer>(sql, new { Count = count });
            
            _logger.LogInformation("Retrieved {Count} top spending customers", customers.Count());
            return customers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting top spending customers: {Count}", count);
            throw;
        }
    }

    public async Task<int> GetNewCustomerCountAsync(DateTime from, DateTime to)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Customers 
                WHERE CreatedAt >= @From AND CreatedAt <= @To";

            using var connection = await _context.GetConnectionAsync();
            var count = await connection.ExecuteScalarAsync<int>(sql, new { From = from, To = to });
            
            _logger.LogInformation("New customer count from {From} to {To}: {Count}", from, to, count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting new customer count from {From} to {To}", from, to);
            throw;
        }
    }
}
