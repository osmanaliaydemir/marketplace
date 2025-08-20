using Application.DTOs.Orders;

namespace Application.Abstractions;

public interface IOrderService
{
    // CRUD Operations
    Task<OrderDetailDto> CreateAsync(OrderCreateRequest request);
    Task<OrderDetailDto> UpdateAsync(long id, OrderUpdateRequest request);
    Task<bool> DeleteAsync(long id);
    Task<OrderDetailDto?> GetByIdAsync(long id);
    Task<OrderDetailDto?> GetByOrderNumberAsync(string orderNumber);
    
    // Customer Orders
    Task<OrderListResponse> GetCustomerOrdersAsync(long customerId, OrderListRequest request);
    Task<OrderDetailDto?> GetCustomerOrderAsync(long customerId, long orderId);
    
    // Store Orders
    Task<OrderListResponse> GetStoreOrdersAsync(long storeId, OrderListRequest request);
    Task<OrderDetailDto?> GetStoreOrderAsync(long storeId, long orderId);
    
    // Order Status Management
    Task<bool> UpdateStatusAsync(long id, string status, string? note = null);
    Task<bool> ConfirmOrderAsync(long id);
    Task<bool> ProcessOrderAsync(long id);
    Task<bool> ShipOrderAsync(long id, string trackingNumber);
    Task<bool> DeliverOrderAsync(long id);
    Task<bool> CancelOrderAsync(long id, string reason);
    
    // Order Groups (Multi-Vendor)
    Task<OrderGroupDto> CreateOrderGroupAsync(OrderGroupCreateRequest request);
    Task<OrderGroupDto?> GetOrderGroupAsync(long orderGroupId);
    Task<IEnumerable<OrderGroupDto>> GetCustomerOrderGroupsAsync(long customerId);
    
    // Payment Integration
    Task<bool> ProcessPaymentAsync(long orderId, PaymentProcessRequest request);
    Task<bool> RefundPaymentAsync(long orderId, RefundRequest request);
    Task<PaymentStatusDto> GetPaymentStatusAsync(long orderId);
    
    // Dashboard and Admin
    Task<OrderStatsDto> GetStatsAsync();
    Task<OrderStatsDto> GetStoreStatsAsync(long storeId);
    Task<IEnumerable<OrderDto>> GetPendingOrdersAsync();
    Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime from, DateTime to);
}
