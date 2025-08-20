using Domain.Entities;

namespace Application.Abstractions;

public interface ISellerRepository : IRepository<Seller>
{
    Task<Seller?> GetByUserIdAsync(long userId);
    Task<Seller?> GetByStoreIdAsync(long storeId);
    Task<IEnumerable<Seller>> GetActiveSellersAsync();
    Task<bool> IsEmailUniqueAsync(string email, long? excludeId = null);
}
