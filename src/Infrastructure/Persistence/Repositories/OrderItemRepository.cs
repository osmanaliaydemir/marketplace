using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.Extensions.Logging;
using Dapper;
using Infrastructure.Persistence.Naming;

namespace Infrastructure.Persistence.Repositories;

public sealed class OrderItemRepository : Repository<OrderItem>, IOrderItemRepository
{
    private readonly IDbContext _context;
    private readonly ILogger<OrderItemRepository> _logger;

    public OrderItemRepository(IDbContext context, ILogger<OrderItemRepository> logger, ITableNameResolver tableNameResolver, IColumnNameResolver columnNameResolver) 
        : base(context, logger, tableNameResolver, columnNameResolver)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderItem>> GetByOrderAsync(long orderId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM OrderItems 
                WHERE OrderId = @OrderId";

            using var connection = await _context.GetConnectionAsync();
            var orderItems = await connection.QueryAsync<OrderItem>(sql, new { OrderId = orderId });
            
            _logger.LogInformation("Order items retrieved for order: {OrderId}, Count: {Count}", orderId, orderItems.Count());
            return orderItems;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order items for order: {OrderId}", orderId);
            throw;
        }
    }

    public async Task<IEnumerable<OrderItem>> GetByProductAsync(long productId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM OrderItems 
                WHERE ProductId = @ProductId";

            using var connection = await _context.GetConnectionAsync();
            var orderItems = await connection.QueryAsync<OrderItem>(sql, new { ProductId = productId });
            
            _logger.LogInformation("Order items retrieved for product: {ProductId}, Count: {Count}", productId, orderItems.Count());
            return orderItems;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order items for product: {ProductId}", productId);
            throw;
        }
    }

    public async Task<decimal> GetTotalAmountByOrderAsync(long orderId)
    {
        try
        {
            const string sql = @"
                SELECT ISNULL(SUM(TotalPrice), 0) FROM OrderItems 
                WHERE OrderId = @OrderId";

            using var connection = await _context.GetConnectionAsync();
            var totalAmount = await connection.QueryFirstOrDefaultAsync<decimal>(sql, new { OrderId = orderId });
            
            _logger.LogInformation("Total amount retrieved for order: {OrderId}, Amount: {Amount}", orderId, totalAmount);
            return totalAmount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total amount for order: {OrderId}", orderId);
            throw;
        }
    }

    public async Task<int> GetTotalQuantityByOrderAsync(long orderId)
    {
        try
        {
            const string sql = @"
                SELECT ISNULL(SUM(Quantity), 0) FROM OrderItems 
                WHERE OrderId = @OrderId";

            using var connection = await _context.GetConnectionAsync();
            var totalQuantity = await connection.QueryFirstOrDefaultAsync<int>(sql, new { OrderId = orderId });
            
            _logger.LogInformation("Total quantity retrieved for order: {OrderId}, Quantity: {Quantity}", orderId, totalQuantity);
            return totalQuantity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total quantity for order: {OrderId}", orderId);
            throw;
        }
    }
}
