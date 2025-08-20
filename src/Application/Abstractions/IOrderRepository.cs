using Domain.Entities;

namespace Application.Abstractions;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByOrderNumberAsync(string orderNumber);
    Task<IEnumerable<Order>> GetByCustomerAsync(long customerId);
    Task<IEnumerable<Order>> GetByStoreAsync(long storeId);
    Task<IEnumerable<Order>> GetByStatusAsync(string status);
    Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<int> GetOrderCountByCustomerAsync(long customerId);
    Task<int> GetOrderCountByStoreAsync(long storeId);
    Task<decimal> GetTotalRevenueByStoreAsync(long storeId, DateTime? startDate = null, DateTime? endDate = null);
}
