using Domain.Entities;

namespace Application.Abstractions;

public interface IOrderItemRepository : IRepository<OrderItem>
{
    Task<IEnumerable<OrderItem>> GetByOrderAsync(long orderId);
    Task<IEnumerable<OrderItem>> GetByProductAsync(long productId);
    Task<decimal> GetTotalAmountByOrderAsync(long orderId);
    Task<int> GetTotalQuantityByOrderAsync(long orderId);
}
