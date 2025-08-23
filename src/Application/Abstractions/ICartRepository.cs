using Domain.Entities;

namespace Application.Abstractions;

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetByCustomerIdAsync(long customerId);
    Task<Cart?> GetBySessionIdAsync(string sessionId);
    Task<IEnumerable<Cart>> GetExpiredCartsAsync();
    Task<bool> ClearCustomerCartAsync(long customerId);
    Task<bool> UpdateCartExpiryAsync(long cartId, DateTime newExpiry);
    Task<IEnumerable<Cart>> GetAbandonedCartsAsync(TimeSpan threshold);
}
