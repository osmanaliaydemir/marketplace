using Application.DTOs.Cart;
using Application.DTOs.Orders;

namespace Application.Abstractions;

public interface ICartService
{
    // Cart Management
    Task<CartDto> GetCartAsync(long customerId);
    Task<CartDto> CreateCartAsync(long customerId);
    Task<bool> ClearCartAsync(long customerId);
    
    // Cart Items
    Task<CartItemDto> AddItemAsync(long customerId, CartAddItemRequest request);
    Task<CartItemDto> UpdateItemAsync(long customerId, long itemId, CartUpdateItemRequest request);
    Task<bool> RemoveItemAsync(long customerId, long itemId);
    Task<bool> UpdateItemQuantityAsync(long customerId, long itemId, int quantity);
    
    // Multi-Vendor Cart
    Task<IEnumerable<CartStoreGroupDto>> GetCartByStoresAsync(long customerId);
    Task<CartStoreGroupDto> GetCartForStoreAsync(long customerId, long storeId);
    
    // Cart Validation
    Task<CartValidationResult> ValidateCartAsync(long customerId);
    Task<bool> CheckStockAvailabilityAsync(long customerId);
    Task<bool> UpdatePricesAsync(long customerId);
    
    // Cart to Order
    Task<OrderCreateRequest> PrepareOrderFromCartAsync(long customerId, CartCheckoutRequest request);
    Task<bool> ReserveCartItemsAsync(long customerId);
    Task<bool> ReleaseCartItemsAsync(long customerId);
    
    // Cart Analytics
    Task<CartStatsDto> GetCartStatsAsync(long customerId);
    Task<IEnumerable<CartAbandonmentDto>> GetAbandonedCartsAsync();
    
    // Session Management
    Task<bool> MergeGuestCartAsync(string sessionId, long customerId);
    Task<bool> TransferCartAsync(long fromCustomerId, long toCustomerId);
}
