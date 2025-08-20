using Application.DTOs.Inventory;
using Application.DTOs.Products;

namespace Application.Abstractions;

public interface IInventoryService
{
    // Stock Management
    Task<bool> UpdateStockAsync(long productId, int quantity);
    Task<bool> ReserveStockAsync(long productId, int quantity);
    Task<bool> ReleaseStockAsync(long productId, int quantity);
    Task<bool> AdjustStockAsync(long productId, int adjustment, string reason);
    
    // Stock Queries
    Task<int> GetCurrentStockAsync(long productId);
    Task<int> GetAvailableStockAsync(long productId);
    Task<int> GetReservedStockAsync(long productId);
    Task<StockHistoryDto> GetStockHistoryAsync(long productId, DateTime from, DateTime to);
    
    // Low Stock Alerts
    Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(int threshold = 10);
    Task<bool> SetLowStockAlertAsync(long productId, int threshold);
    Task<bool> RemoveLowStockAlertAsync(long productId);
    
    // Stock Reservations
    Task<StockReservationDto> CreateReservationAsync(StockReservationRequest request);
    Task<bool> ExtendReservationAsync(long reservationId, TimeSpan extension);
    Task<bool> CancelReservationAsync(long reservationId);
    Task<bool> ConfirmReservationAsync(long reservationId);
    
    // Inventory Reports
    Task<InventoryStatsDto> GetInventoryStatsAsync();
    Task<InventoryStatsDto> GetStoreInventoryStatsAsync(long storeId);
    Task<IEnumerable<InventoryMovementDto>> GetInventoryMovementsAsync(DateTime from, DateTime to);
    
    // Bulk Operations
    Task<bool> BulkUpdateStockAsync(IEnumerable<StockUpdateRequest> requests);
    Task<bool> BulkReserveStockAsync(IEnumerable<StockReservationRequest> requests);
    Task<bool> BulkReleaseStockAsync(IEnumerable<long> reservationIds);
    
    // Inventory Validation
    Task<bool> ValidateStockOperationAsync(StockOperationRequest request);
    Task<bool> CheckStockAvailabilityAsync(long productId, int quantity);
    Task<StockValidationResult> ValidateInventoryAsync(long productId);
}
