using Domain.Entities;

namespace Application.Abstractions;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByProviderPaymentIdAsync(string providerPaymentId);
    Task<IEnumerable<Payment>> GetByOrderAsync(long orderId);
    Task<IEnumerable<Payment>> GetByCustomerAsync(long customerId);
    Task<IEnumerable<Payment>> GetByStatusAsync(string status);
    Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to);
    Task<int> GetSuccessfulPaymentCountAsync(DateTime from, DateTime to);
    
    // Refund methods
    Task<Refund> AddRefundAsync(Refund refund);
    Task<Refund?> GetRefundByIdAsync(string refundId);
    
    // Payment Split methods
    Task<PaymentSplit> AddPaymentSplitAsync(PaymentSplit paymentSplit);
    Task<PaymentSplit?> GetPaymentSplitByPaymentIdAsync(string paymentId);
    Task<bool> UpdatePaymentSplitAsync(PaymentSplit paymentSplit);
}
