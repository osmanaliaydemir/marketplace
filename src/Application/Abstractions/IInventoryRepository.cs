using Domain.Entities;

namespace Application.Abstractions;

public interface IInventoryRepository : IRepository<Inventory>
{
    Task<Inventory?> GetByProductIdAsync(long productId);
    Task<IEnumerable<Inventory>> GetLowStockAsync(int threshold = 10);
    Task<IEnumerable<Inventory>> GetOutOfStockAsync();
    Task<bool> UpdateStockAsync(long productId, int newStock);
    Task<bool> ReserveStockAsync(long productId, int quantity);
    Task<bool> ReleaseStockAsync(long productId, int quantity);
    Task<int> GetAvailableStockAsync(long productId);
    Task<int> GetReservedStockAsync(long productId);
}
