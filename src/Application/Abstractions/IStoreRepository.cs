using Domain.Entities;

namespace Application.Abstractions;

public interface IStoreRepository : IRepository<Store>
{
    Task<Store?> GetBySlugAsync(string slug);
    Task<Store?> GetByUserIdAsync(long userId);
    Task<IEnumerable<Store>> GetActiveStoresAsync();
    Task<IEnumerable<Store>> GetFeaturedStoresAsync();
    Task<bool> IsSlugUniqueAsync(string slug, long? excludeId = null);
}
