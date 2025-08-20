using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.Extensions.Logging;
using Dapper;
using Infrastructure.Persistence.Naming;

namespace Infrastructure.Persistence.Repositories;

public sealed class OrderRepository : Repository<Order>, IOrderRepository
{
    private readonly IDbContext _context;
    private readonly ILogger<OrderRepository> _logger;

    public OrderRepository(IDbContext context, ILogger<OrderRepository> logger, ITableNameResolver tableNameResolver, IColumnNameResolver columnNameResolver) 
        : base(context, logger, tableNameResolver, columnNameResolver)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Orders 
                WHERE OrderNumber = @OrderNumber";

            using var connection = await _context.GetConnectionAsync();
            var order = await connection.QueryFirstOrDefaultAsync<Order>(sql, new { OrderNumber = orderNumber });
            
            _logger.LogInformation("Order retrieved by order number: {OrderNumber}", orderNumber);
            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order by order number: {OrderNumber}", orderNumber);
            throw;
        }
    }

    public async Task<IEnumerable<Order>> GetByCustomerAsync(long customerId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Orders 
                WHERE CustomerId = @CustomerId 
                ORDER BY CreatedAt DESC";

            using var connection = await _context.GetConnectionAsync();
            var orders = await connection.QueryAsync<Order>(sql, new { CustomerId = customerId });
            
            _logger.LogInformation("Retrieved {Count} orders for customer: {CustomerId}", orders.Count(), customerId);
            return orders;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders by customer: {CustomerId}", customerId);
            throw;
        }
    }

    public async Task<IEnumerable<Order>> GetByStoreAsync(long storeId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Orders 
                WHERE StoreId = @StoreId 
                ORDER BY CreatedAt DESC";

            using var connection = await _context.GetConnectionAsync();
            var orders = await connection.QueryAsync<Order>(sql, new { StoreId = storeId });
            
            _logger.LogInformation("Retrieved {Count} orders for store: {StoreId}", orders.Count(), storeId);
            return orders;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders by store: {StoreId}", storeId);
            throw;
        }
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(string status)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Orders 
                WHERE Status = @Status 
                ORDER BY CreatedAt DESC";

            using var connection = await _context.GetConnectionAsync();
            var orders = await connection.QueryAsync<Order>(sql, new { Status = status });
            
            _logger.LogInformation("Retrieved {Count} orders with status: {Status}", orders.Count(), status);
            return orders;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders by status: {Status}", status);
            throw;
        }
    }

    public async Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Orders 
                WHERE CreatedAt >= @From AND CreatedAt <= @To 
                ORDER BY CreatedAt DESC";

            using var connection = await _context.GetConnectionAsync();
            var orders = await connection.QueryAsync<Order>(sql, new { From = from, To = to });
            
            _logger.LogInformation("Retrieved {Count} orders from {From} to {To}", orders.Count(), from, to);
            return orders;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders by date range: {From} to {To}", from, to);
            throw;
        }
    }

    public async Task<int> GetTotalOrderCountAsync()
    {
        try
        {
            const string sql = "SELECT COUNT(*) FROM Orders";

            using var connection = await _context.GetConnectionAsync();
            var count = await connection.ExecuteScalarAsync<int>(sql);
            
            _logger.LogInformation("Total order count: {Count}", count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total order count");
            throw;
        }
    }

    public async Task<int> GetOrderCountByStatusAsync(string status)
    {
        try
        {
            const string sql = "SELECT COUNT(*) FROM Orders WHERE Status = @Status";

            using var connection = await _context.GetConnectionAsync();
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Status = status });
            
            _logger.LogInformation("Order count for status {Status}: {Count}", status, count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order count by status: {Status}", status);
            throw;
        }
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to)
    {
        try
        {
            const string sql = @"
                SELECT COALESCE(SUM(TotalAmount), 0) 
                FROM Orders 
                WHERE Status = 'Completed' 
                AND CreatedAt >= @From AND CreatedAt <= @To";

            using var connection = await _context.GetConnectionAsync();
            var revenue = await connection.ExecuteScalarAsync<decimal>(sql, new { From = from, To = to });
            
            _logger.LogInformation("Total revenue from {From} to {To}: {Revenue}", from, to, revenue);
            return revenue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total revenue from {From} to {To}", from, to);
            throw;
        }
    }

    public async Task<decimal> GetStoreRevenueAsync(long storeId, DateTime from, DateTime to)
    {
        try
        {
            const string sql = @"
                SELECT COALESCE(SUM(TotalAmount), 0) 
                FROM Orders 
                WHERE StoreId = @StoreId 
                AND Status = 'Completed' 
                AND CreatedAt >= @From AND CreatedAt <= @To";

            using var connection = await _context.GetConnectionAsync();
            var revenue = await connection.ExecuteScalarAsync<decimal>(sql, new { StoreId = storeId, From = from, To = to });
            
            _logger.LogInformation("Store {StoreId} revenue from {From} to {To}: {Revenue}", storeId, from, to, revenue);
            return revenue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting store revenue for store {StoreId} from {From} to {To}", storeId, from, to);
            throw;
        }
    }

    public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int count = 10)
    {
        try
        {
            const string sql = @"
                SELECT TOP(@Count) * FROM Orders 
                ORDER BY CreatedAt DESC";

            using var connection = await _context.GetConnectionAsync();
            var orders = await connection.QueryAsync<Order>(sql, new { Count = count });
            
            _logger.LogInformation("Retrieved {Count} recent orders", orders.Count());
            return orders;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent orders: {Count}", count);
            throw;
        }
    }

    public async Task<int> GetOrderCountByCustomerAsync(long customerId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Orders 
                WHERE CustomerId = @CustomerId";

            using var connection = await _context.GetConnectionAsync();
            var count = await connection.ExecuteScalarAsync<int>(sql, new { CustomerId = customerId });
            
            _logger.LogInformation("Order count retrieved for customer: {CustomerId}, Count: {Count}", customerId, count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order count for customer: {CustomerId}", customerId);
            throw;
        }
    }

    public async Task<int> GetOrderCountByStoreAsync(long storeId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Orders 
                WHERE StoreId = @StoreId";

            using var connection = await _context.GetConnectionAsync();
            var count = await connection.ExecuteScalarAsync<int>(sql, new { StoreId = storeId });
            
            _logger.LogInformation("Order count retrieved for store: {StoreId}, Count: {Count}", storeId, count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order count for store: {StoreId}", storeId);
            throw;
        }
    }

    public async Task<decimal> GetTotalRevenueByStoreAsync(long storeId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var sql = @"
                SELECT COALESCE(SUM(TotalAmount), 0) FROM Orders 
                WHERE StoreId = @StoreId AND Status = 'completed'";

            object parameters;

            if (startDate.HasValue && endDate.HasValue)
            {
                sql += " AND CreatedAt >= @StartDate AND CreatedAt <= @EndDate";
                parameters = new { StoreId = storeId, StartDate = startDate.Value, EndDate = endDate.Value };
            }
            else if (startDate.HasValue)
            {
                sql += " AND CreatedAt >= @StartDate";
                parameters = new { StoreId = storeId, StartDate = startDate.Value };
            }
            else if (endDate.HasValue)
            {
                sql += " AND CreatedAt <= @EndDate";
                parameters = new { StoreId = storeId, EndDate = endDate.Value };
            }
            else
            {
                parameters = new { StoreId = storeId };
            }

            using var connection = await _context.GetConnectionAsync();
            var revenue = await connection.ExecuteScalarAsync<decimal>(sql, parameters);
            
            _logger.LogInformation("Total revenue retrieved for store: {StoreId}, Revenue: {Revenue}", storeId, revenue);
            return revenue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total revenue for store: {StoreId}", storeId);
            throw;
        }
    }
}
