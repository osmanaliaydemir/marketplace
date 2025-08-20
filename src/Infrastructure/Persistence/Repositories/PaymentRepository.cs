using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.Extensions.Logging;
using Dapper;
using Infrastructure.Persistence.Naming;

namespace Infrastructure.Persistence.Repositories;

public sealed class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    private readonly IDbContext _context;
    private readonly ILogger<PaymentRepository> _logger;

    public PaymentRepository(IDbContext context, ILogger<PaymentRepository> logger, ITableNameResolver tableNameResolver, IColumnNameResolver columnNameResolver) : base(context, logger, tableNameResolver, columnNameResolver)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Payment?> GetByProviderPaymentIdAsync(string providerPaymentId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Payments 
                WHERE ProviderPaymentId = @ProviderPaymentId";

            using var connection = await _context.GetConnectionAsync();
            var payment = await connection.QueryFirstOrDefaultAsync<Payment>(sql, new { ProviderPaymentId = providerPaymentId });
            
            _logger.LogInformation("Payment retrieved by provider payment ID: {ProviderPaymentId}", providerPaymentId);
            return payment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment by provider payment ID: {ProviderPaymentId}", providerPaymentId);
            throw;
        }
    }

    public async Task<IEnumerable<Payment>> GetByOrderAsync(long orderId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Payments 
                WHERE OrderId = @OrderId";

            using var connection = await _context.GetConnectionAsync();
            var payments = await connection.QueryAsync<Payment>(sql, new { OrderId = orderId });
            
            _logger.LogInformation("Retrieved {Count} payments for order: {OrderId}", payments.Count(), orderId);
            return payments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payments by order: {OrderId}", orderId);
            throw;
        }
    }

    public async Task<IEnumerable<Payment>> GetByCustomerAsync(long customerId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Payments 
                WHERE CustomerId = @CustomerId 
                ORDER BY CreatedAt DESC";

            using var connection = await _context.GetConnectionAsync();
            var payments = await connection.QueryAsync<Payment>(sql, new { CustomerId = customerId });
            
            _logger.LogInformation("Retrieved {Count} payments for customer: {CustomerId}", payments.Count(), customerId);
            return payments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payments by customer: {CustomerId}", customerId);
            throw;
        }
    }

    public async Task<IEnumerable<Payment>> GetByStatusAsync(string status)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Payments 
                WHERE Status = @Status 
                ORDER BY CreatedAt DESC";

            using var connection = await _context.GetConnectionAsync();
            var payments = await connection.QueryAsync<Payment>(sql, new { Status = status });
            
            _logger.LogInformation("Retrieved {Count} payments with status: {Status}", payments.Count(), status);
            return payments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payments by status: {Status}", status);
            throw;
        }
    }

    public async Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Payments 
                WHERE CreatedAt >= @From AND CreatedAt <= @To 
                ORDER BY CreatedAt DESC";

            using var connection = await _context.GetConnectionAsync();
            var payments = await connection.QueryAsync<Payment>(sql, new { From = from, To = to });
            
            _logger.LogInformation("Retrieved {Count} payments from {From} to {To}", payments.Count(), from, to);
            return payments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payments by date range: {From} to {To}", from, to);
            throw;
        }
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to)
    {
        try
        {
            const string sql = @"
                SELECT COALESCE(SUM(Amount), 0) 
                FROM Payments 
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

    public async Task<int> GetSuccessfulPaymentCountAsync(DateTime from, DateTime to)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) 
                FROM Payments 
                WHERE Status = 'Completed' 
                AND CreatedAt >= @From AND CreatedAt <= @To";

            using var connection = await _context.GetConnectionAsync();
            var count = await connection.ExecuteScalarAsync<int>(sql, new { From = from, To = to });
            
            _logger.LogInformation("Successful payment count from {From} to {To}: {Count}", from, to, count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting successful payment count from {From} to {To}", from, to);
            throw;
        }
    }

    public async Task<Refund> AddRefundAsync(Refund refund)
    {
        try
        {
            const string sql = @"
                INSERT INTO Refunds (PaymentId, Amount, Currency, Reason, Status, CreatedAt)
                VALUES (@PaymentId, @Amount, @Currency, @Reason, @Status, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as bigint)";

            using var connection = await _context.GetConnectionAsync();
            var refundId = await connection.ExecuteScalarAsync<long>(sql, new
            {
                refund.PaymentId,
                refund.Amount,
                refund.Currency,
                refund.Reason,
                Status = refund.Status.ToString(),
                refund.CreatedAt
            });

            _logger.LogInformation("Refund added successfully: {RefundId}", refundId);
            return new Refund
            {
                Id = refundId,
                PaymentId = refund.PaymentId,
                Amount = refund.Amount,
                Reason = refund.Reason,
                Status = refund.Status,
                CreatedAt = refund.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding refund for payment: {PaymentId}", refund.PaymentId);
            throw;
        }
    }

    public async Task<Refund?> GetRefundByIdAsync(string refundId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Refunds 
                WHERE Id = @RefundId";

            using var connection = await _context.GetConnectionAsync();
            var refund = await connection.QueryFirstOrDefaultAsync<Refund>(sql, new { RefundId = refundId });
            
            _logger.LogInformation("Refund retrieved by ID: {RefundId}", refundId);
            return refund;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting refund by ID: {RefundId}", refundId);
            throw;
        }
    }

    public async Task<PaymentSplit> AddPaymentSplitAsync(PaymentSplit paymentSplit)
    {
        try
        {
            const string sql = @"
                INSERT INTO PaymentSplits (PaymentId, StoreId, TotalAmount, CommissionAmount, StoreAmount, CommissionRate, Status, CreatedAt)
                VALUES (@PaymentId, @StoreId, @TotalAmount, @CommissionAmount, @StoreAmount, @CommissionRate, @Status, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as bigint)";

            using var connection = await _context.GetConnectionAsync();
            var splitId = await connection.ExecuteScalarAsync<long>(sql, new
            {
                paymentSplit.PaymentId,
                paymentSplit.StoreId,
                paymentSplit.TotalAmount,
                paymentSplit.CommissionAmount,
                paymentSplit.StoreAmount,
                paymentSplit.CommissionRate,
                Status = paymentSplit.Status.ToString(),
                paymentSplit.CreatedAt
            });

            _logger.LogInformation("Payment split added successfully: {SplitId}", splitId);
            return new PaymentSplit
            {
                Id = splitId,
                PaymentId = paymentSplit.PaymentId,
                StoreId = paymentSplit.StoreId,
                TotalAmount = paymentSplit.TotalAmount,
                CommissionAmount = paymentSplit.CommissionAmount,
                StoreAmount = paymentSplit.StoreAmount,
                CommissionRate = paymentSplit.CommissionRate,
                Status = paymentSplit.Status,
                CreatedAt = paymentSplit.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding payment split for payment: {PaymentId}", paymentSplit.PaymentId);
            throw;
        }
    }

    public async Task<PaymentSplit?> GetPaymentSplitByPaymentIdAsync(string paymentId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PaymentSplits 
                WHERE PaymentId = @PaymentId";

            using var connection = await _context.GetConnectionAsync();
            var paymentSplit = await connection.QueryFirstOrDefaultAsync<PaymentSplit>(sql, new { PaymentId = paymentId });
            
            _logger.LogInformation("Payment split retrieved for payment: {PaymentId}", paymentId);
            return paymentSplit;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment split for payment: {PaymentId}", paymentId);
            throw;
        }
    }

    public async Task<bool> UpdatePaymentSplitAsync(PaymentSplit paymentSplit)
    {
        try
        {
            const string sql = @"
                UPDATE PaymentSplits 
                SET Status = @Status, CompletedAt = @CompletedAt
                WHERE Id = @Id";

            using var connection = await _context.GetConnectionAsync();
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                paymentSplit.Id,
                Status = paymentSplit.Status.ToString(),
                paymentSplit.CompletedAt
            });

            var success = rowsAffected > 0;
            _logger.LogInformation("Payment split updated: {SplitId}, Success: {Success}", paymentSplit.Id, success);
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment split: {SplitId}", paymentSplit.Id);
            throw;
        }
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByMethodAsync(string paymentMethod, DateTime from, DateTime to)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Payments 
                WHERE PaymentMethod = @PaymentMethod 
                AND CreatedAt >= @From AND CreatedAt <= @To 
                ORDER BY CreatedAt DESC";

            using var connection = await _context.GetConnectionAsync();
            var payments = await connection.QueryAsync<Payment>(sql, new { PaymentMethod = paymentMethod, From = from, To = to });
            
            _logger.LogInformation("Retrieved {Count} payments by method {Method} from {From} to {To}", 
                payments.Count(), paymentMethod, from, to);
            return payments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payments by method {Method} from {From} to {To}", paymentMethod, from, to);
            throw;
        }
    }

    public async Task<decimal> GetAveragePaymentAmountAsync(DateTime from, DateTime to)
    {
        try
        {
            const string sql = @"
                SELECT COALESCE(AVG(Amount), 0) 
                FROM Payments 
                WHERE Status = 'Completed' 
                AND CreatedAt >= @From AND CreatedAt <= @To";

            using var connection = await _context.GetConnectionAsync();
            var avgAmount = await connection.ExecuteScalarAsync<decimal>(sql, new { From = from, To = to });
            
            _logger.LogInformation("Average payment amount from {From} to {To}: {AvgAmount}", from, to, avgAmount);
            return avgAmount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting average payment amount from {From} to {To}", from, to);
            throw;
        }
    }

    public async Task<IEnumerable<Payment>> GetFailedPaymentsAsync(DateTime from, DateTime to)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Payments 
                WHERE Status = 'Failed' 
                AND CreatedAt >= @From AND CreatedAt <= @To 
                ORDER BY CreatedAt DESC";

            using var connection = await _context.GetConnectionAsync();
            var payments = await connection.QueryAsync<Payment>(sql, new { From = from, To = to });
            
            _logger.LogInformation("Retrieved {Count} failed payments from {From} to {To}", payments.Count(), from, to);
            return payments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting failed payments from {From} to {To}", from, to);
            throw;
        }
    }
}
