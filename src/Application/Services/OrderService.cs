using Application.Abstractions;
using Application.DTOs.Orders;
using Application.DTOs.Customers;
using Application.Exceptions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public sealed class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IStoreRepository _storeRepository;

    public OrderService(
        ILogger<OrderService> logger,
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        IStoreRepository storeRepository)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _storeRepository = storeRepository;
    }

    public async Task<OrderDetailDto> CreateAsync(OrderCreateRequest request)
    {
        _logger.LogInformation("Creating new order for customer: {CustomerId}", request.CustomerId);
        
        try
        {
            // Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            if (customer == null)
            {
                _logger.LogWarning("Customer not found: {CustomerId}", request.CustomerId);
                throw new ArgumentException($"Customer with ID {request.CustomerId} not found");
            }

            // Validate store exists
            var store = await _storeRepository.GetByIdAsync(request.StoreId);
            if (store == null)
            {
                _logger.LogWarning("Store not found: {StoreId}", request.StoreId);
                throw new ArgumentException($"Store with ID {request.StoreId} not found");
            }

            // Generate order number
            var orderNumber = GenerateOrderNumber();
            
            // Create order entity
            var order = new Order
            {
                OrderNumber = orderNumber,
                CustomerId = request.CustomerId,
                StoreId = request.StoreId,
                Status = OrderStatus.Pending,
                Subtotal = request.SubTotal,
                TaxAmount = request.TaxAmount,
                ShippingAmount = request.ShippingAmount,
                DiscountAmount = request.DiscountAmount,
                TotalAmount = request.TotalAmount,
                Currency = request.Currency ?? "TRY",
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };
            
            var createdOrder = await _orderRepository.AddAsync(order);
            _logger.LogInformation("Order created successfully: {OrderId}, OrderNumber: {OrderNumber}", 
                createdOrder.Id, createdOrder.OrderNumber);
            
            return await MapToOrderDetailDtoAsync(createdOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order for customer: {CustomerId}", request.CustomerId);
            throw;
        }
    }

    public async Task<OrderDetailDto> UpdateAsync(long id, OrderUpdateRequest request)
    {
        _logger.LogInformation("Updating order ID: {OrderId}", id);
        
        try
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order not found: {OrderId}", id);
                throw new ArgumentException($"Order with ID {id} not found");
            }

            // Update order properties
            if (request.Notes != null)
                order.Notes = request.Notes;
            
            if (request.Notes != null)
                order.Notes = request.Notes;

            order.ModifiedAt = DateTime.UtcNow;
            
            await _orderRepository.UpdateAsync(order);
            _logger.LogInformation("Order updated successfully: {OrderId}", id);
            
            return await MapToOrderDetailDtoAsync(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order: {OrderId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long id)
    {
        _logger.LogInformation("Deleting order ID: {OrderId}", id);
        
        try
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order not found for deletion: {OrderId}", id);
                return false;
            }

            // Only allow deletion of pending orders
            if (order.Status != OrderStatus.Pending)
            {
                _logger.LogWarning("Cannot delete order with status {Status}: {OrderId}", order.Status, id);
                return false;
            }

            await _orderRepository.DeleteAsync(id);
            _logger.LogInformation("Order deleted successfully: {OrderId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting order: {OrderId}", id);
            throw;
        }
    }

    public async Task<OrderDetailDto?> GetByIdAsync(long id)
    {
        _logger.LogInformation("Getting order by ID: {OrderId}", id);
        
        try
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order not found: {OrderId}", id);
                return null;
            }

            return await MapToOrderDetailDtoAsync(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order by ID: {OrderId}", id);
            throw;
        }
    }

    public async Task<OrderDetailDto?> GetByOrderNumberAsync(string orderNumber)
    {
        _logger.LogInformation("Getting order by order number: {OrderNumber}", orderNumber);
        
        try
        {
            var order = await _orderRepository.GetByOrderNumberAsync(orderNumber);
            if (order == null)
            {
                _logger.LogWarning("Order not found by order number: {OrderNumber}", orderNumber);
                return null;
            }

            return await MapToOrderDetailDtoAsync(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order by order number: {OrderNumber}", orderNumber);
            throw;
        }
    }

    public async Task<OrderListResponse> GetCustomerOrdersAsync(long customerId, OrderListRequest request)
    {
        _logger.LogInformation("Getting orders for customer: {CustomerId}", customerId);
        
        try
        {
            var orders = await _orderRepository.GetByCustomerAsync(customerId);
            var orderDtos = await MapToOrderListDtosAsync(orders);
            
            // Apply pagination
            var totalCount = orderDtos.Count();
            var pagedOrders = orderDtos
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new OrderListResponse
            {
                Orders = pagedOrders,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer orders: {CustomerId}", customerId);
            throw;
        }
    }

    public async Task<OrderDetailDto?> GetCustomerOrderAsync(long customerId, long orderId)
    {
        _logger.LogInformation("Getting customer order: CustomerId: {CustomerId}, OrderId: {OrderId}", customerId, orderId);
        
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null || order.CustomerId != customerId)
            {
                _logger.LogWarning("Order not found or customer mismatch: CustomerId: {CustomerId}, OrderId: {OrderId}", customerId, orderId);
                return null;
            }

            return await MapToOrderDetailDtoAsync(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer order: CustomerId: {CustomerId}, OrderId: {OrderId}", customerId, orderId);
            throw;
        }
    }

    public async Task<OrderListResponse> GetStoreOrdersAsync(long storeId, OrderListRequest request)
    {
        _logger.LogInformation("Getting orders for store: {StoreId}", storeId);
        
        try
        {
            var orders = await _orderRepository.GetByStoreAsync(storeId);
            var orderDtos = await MapToOrderListDtosAsync(orders);
            
            // Apply pagination
            var totalCount = orderDtos.Count();
            var pagedOrders = orderDtos
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new OrderListResponse
            {
                Orders = pagedOrders,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting store orders: {StoreId}", storeId);
            throw;
        }
    }

    public async Task<OrderDetailDto?> GetStoreOrderAsync(long storeId, long orderId)
    {
        _logger.LogInformation("Getting store order: StoreId: {StoreId}, OrderId: {OrderId}", storeId, orderId);
        
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null || order.StoreId != storeId)
            {
                _logger.LogWarning("Order not found or store mismatch: StoreId: {StoreId}, OrderId: {OrderId}", storeId, orderId);
                return null;
            }

            return await MapToOrderDetailDtoAsync(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting store order: StoreId: {StoreId}, OrderId: {OrderId}", storeId, orderId);
            throw;
        }
    }

    public async Task<bool> UpdateStatusAsync(long id, OrderStatus status, string? note = null)
    {
        _logger.LogInformation("Updating order status: OrderId: {OrderId}, Status: {Status}", id, status);
        
        try
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order not found for status update: {OrderId}", id);
                return false;
            }

            // Validate status transition
            if (!IsValidStatusTransition(order.Status, status))
            {
                _logger.LogWarning("Invalid status transition from {CurrentStatus} to {NewStatus}: {OrderId}", 
                    order.Status, status, id);
                return false;
            }

            order.Status = status;
            order.ModifiedAt = DateTime.UtcNow;
            
            // Update status-specific timestamps
            switch (status)
            {
                case OrderStatus.Shipped:
                    order.ShippedAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Delivered:
                    order.DeliveredAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Cancelled:
                    order.CancelledAt = DateTime.UtcNow;
                    break;
            }

            if (!string.IsNullOrEmpty(note))
                order.Notes = note;

            await _orderRepository.UpdateAsync(order);
            _logger.LogInformation("Order status updated successfully: OrderId: {OrderId}, Status: {Status}", id, status);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order status: OrderId: {OrderId}, Status: {Status}", id, status);
            throw;
        }
    }

    public async Task<bool> ConfirmOrderAsync(long id)
    {
        return await UpdateStatusAsync(id, OrderStatus.Confirmed);
    }

    public async Task<bool> ProcessOrderAsync(long id)
    {
        return await UpdateStatusAsync(id, OrderStatus.Processing);
    }

    public async Task<bool> ShipOrderAsync(long id, string trackingNumber)
    {
        _logger.LogInformation("Shipping order: OrderId: {OrderId}, TrackingNumber: {TrackingNumber}", id, trackingNumber);
        
        try
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order not found for shipping: {OrderId}", id);
                return false;
            }

            // Tracking number is now handled by Shipment entity
            await UpdateStatusAsync(id, OrderStatus.Shipped);
            
            _logger.LogInformation("Order shipped successfully: OrderId: {OrderId}, TrackingNumber: {TrackingNumber}", id, trackingNumber);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error shipping order: OrderId: {OrderId}", id);
            throw;
        }
    }

    public async Task<bool> DeliverOrderAsync(long id)
    {
        return await UpdateStatusAsync(id, OrderStatus.Delivered);
    }

    public async Task<bool> CancelOrderAsync(long id, string reason)
    {
        _logger.LogInformation("Cancelling order: OrderId: {OrderId}, Reason: {Reason}", id, reason);
        
        try
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order not found for cancellation: {OrderId}", id);
                return false;
            }

            // Only allow cancellation of pending or confirmed orders
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
            {
                _logger.LogWarning("Cannot cancel order with status {Status}: {OrderId}", order.Status, id);
                return false;
            }

            order.Notes = reason;
            await UpdateStatusAsync(id, OrderStatus.Cancelled);
            
            _logger.LogInformation("Order cancelled successfully: OrderId: {OrderId}, Reason: {Reason}", id, reason);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order: OrderId: {OrderId}", id);
            throw;
        }
    }

    public async Task<OrderGroupDto> CreateOrderGroupAsync(OrderGroupCreateRequest request)
    {
        _logger.LogInformation("Creating order group for customer: {CustomerId}", request.CustomerId);
        
        try
        {
            // Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            if (customer == null)
            {
                _logger.LogWarning("Customer not found for order group: {CustomerId}", request.CustomerId);
                throw new ArgumentException($"Customer with ID {request.CustomerId} not found");
            }

            // Create order group
            var orderGroup = new OrderGroup
            {
                CustomerId = request.CustomerId,
                Status = "Pending",
                TotalAmount = request.TotalAmount,
                Currency = request.Currency ?? "TRY",
                // Notes are not supported in OrderGroup entity
                CreatedAt = DateTime.UtcNow
            };

            // This would require OrderGroupRepository implementation
            // For now, return a basic DTO
            return new OrderGroupDto
            {
                Id = 0, // Will be set when OrderGroup entity is properly implemented
                CustomerId = request.CustomerId,
                Status = "Pending",
                TotalAmount = request.TotalAmount,
                Currency = request.Currency ?? "TRY",
                CreatedAt = DateTime.UtcNow,
                Orders = new List<OrderDto>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order group for customer: {CustomerId}", request.CustomerId);
            throw;
        }
    }

    public async Task<OrderGroupDto?> GetOrderGroupAsync(long orderGroupId)
    {
        // This would require OrderGroupRepository implementation
        // For now, return null
        _logger.LogWarning("OrderGroup functionality not yet implemented: {OrderGroupId}", orderGroupId);
        return null;
    }

    public async Task<IEnumerable<OrderGroupDto>> GetCustomerOrderGroupsAsync(long customerId)
    {
        // This would require OrderGroupRepository implementation
        // For now, return empty list
        _logger.LogWarning("OrderGroup functionality not yet implemented for customer: {CustomerId}", customerId);
        return new List<OrderGroupDto>();
    }

    public async Task<bool> ProcessPaymentAsync(long orderId, Application.DTOs.Payments.PaymentProcessRequest request)
    {
        _logger.LogInformation("Processing payment for order: {OrderId}", orderId);
        
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order not found for payment processing: {OrderId}", orderId);
                return false;
            }

            // This would integrate with PaymentService
            // For now, just update order status
            await UpdateStatusAsync(orderId, OrderStatus.Confirmed);
            
            _logger.LogInformation("Payment processed successfully for order: {OrderId}", orderId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for order: {OrderId}", orderId);
            throw;
        }
    }

    public async Task<bool> RefundPaymentAsync(long orderId, Application.DTOs.Payments.RefundRequest request)
    {
        _logger.LogInformation("Processing refund for order: {OrderId}", orderId);
        
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order not found for refund: {OrderId}", orderId);
                return false;
            }

            // This would integrate with PaymentService
            // For now, just update order status
            await UpdateStatusAsync(orderId, OrderStatus.Refunded);
            
            _logger.LogInformation("Refund processed successfully for order: {OrderId}", orderId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing refund for order: {OrderId}", orderId);
            throw;
        }
    }

    public async Task<PaymentStatusDto> GetPaymentStatusAsync(long orderId)
    {
        _logger.LogInformation("Getting payment status for order: {OrderId}", orderId);
        
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order not found for payment status: {OrderId}", orderId);
                return new PaymentStatusDto
                {
                    OrderId = orderId,
                    PaymentStatus = "Unknown",
                    ErrorMessage = "Order not found"
                };
            }

            // This would integrate with PaymentService
            // For now, return basic status based on order status
            var paymentStatus = order.Status switch
            {
                OrderStatus.Pending => "Pending",
                OrderStatus.Confirmed => "Pending",
                OrderStatus.Processing => "Processing",
                OrderStatus.Shipped => "Completed",
                OrderStatus.Delivered => "Completed",
                OrderStatus.Cancelled => "Cancelled",
                _ => "Unknown"
            };

            return new PaymentStatusDto
            {
                OrderId = orderId,
                OrderNumber = order.OrderNumber,
                PaymentStatus = paymentStatus,
                PaymentMethod = "Credit Card", // Default value
                Amount = order.TotalAmount,
                Currency = order.Currency,
                CreatedAt = order.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment status for order: {OrderId}", orderId);
            throw;
        }
    }

    public async Task<OrderStatsDto> GetStatsAsync()
    {
        _logger.LogInformation("Getting order statistics");
        
        try
        {
            var allOrders = await _orderRepository.GetAllAsync();
            
            var stats = new OrderStatsDto
            {
                TotalOrders = allOrders.Count(),
                TotalRevenue = allOrders.Sum(o => o.TotalAmount),
                PendingOrders = allOrders.Count(o => o.Status == OrderStatus.Pending),
                ProcessingOrders = allOrders.Count(o => o.Status == OrderStatus.Processing),
                ShippedOrders = allOrders.Count(o => o.Status == OrderStatus.Shipped),
                DeliveredOrders = allOrders.Count(o => o.Status == OrderStatus.Delivered),
                CancelledOrders = allOrders.Count(o => o.Status == OrderStatus.Cancelled),
                AverageOrderValue = allOrders.Any() ? allOrders.Average(o => o.TotalAmount) : 0,
                Currency = "TRY"
            };

            _logger.LogInformation("Order statistics retrieved successfully");
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order statistics");
            throw;
        }
    }

    public async Task<OrderStatsDto> GetStoreStatsAsync(long storeId)
    {
        _logger.LogInformation("Getting order statistics for store: {StoreId}", storeId);
        
        try
        {
            var storeOrders = await _orderRepository.GetByStoreAsync(storeId);
            
            var stats = new OrderStatsDto
            {
                TotalOrders = storeOrders.Count(),
                TotalRevenue = storeOrders.Sum(o => o.TotalAmount),
                            PendingOrders = storeOrders.Count(o => o.Status == Domain.Enums.OrderStatus.Pending),
            ProcessingOrders = storeOrders.Count(o => o.Status == Domain.Enums.OrderStatus.Processing),
            ShippedOrders = storeOrders.Count(o => o.Status == Domain.Enums.OrderStatus.Shipped),
            DeliveredOrders = storeOrders.Count(o => o.Status == Domain.Enums.OrderStatus.Delivered),
            CancelledOrders = storeOrders.Count(o => o.Status == Domain.Enums.OrderStatus.Cancelled),
                AverageOrderValue = storeOrders.Any() ? storeOrders.Average(o => o.TotalAmount) : 0,
                Currency = "TRY"
            };

            _logger.LogInformation("Store order statistics retrieved successfully for store: {StoreId}", storeId);
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting store order statistics for store: {StoreId}", storeId);
            throw;
        }
    }

    public async Task<IEnumerable<OrderDto>> GetPendingOrdersAsync()
    {
        _logger.LogInformation("Getting pending orders");
        
        try
        {
            var pendingOrders = await _orderRepository.GetByStatusAsync(OrderStatus.Pending);
            var orderDtos = await MapToOrderListDtosAsync(pendingOrders);
            
            _logger.LogInformation("Found {Count} pending orders", orderDtos.Count());
            return orderDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending orders");
            throw;
        }
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime from, DateTime to)
    {
        _logger.LogInformation("Getting orders by date range: {From} to {To}", from, to);
        
        try
        {
            var orders = await _orderRepository.GetByDateRangeAsync(from, to);
            var orderDtos = await MapToOrderListDtosAsync(orders);
            
            _logger.LogInformation("Found {Count} orders in date range", orderDtos.Count());
            return orderDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders by date range: {From} to {To}", from, to);
            throw;
        }
    }
    
    #region Private Helper Methods
    
    private string GenerateOrderNumber()
    {
        // Generate unique order number: ORD-YYYYMMDD-XXXX
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random().Next(1000, 9999);
        return $"ORD-{date}-{random}";
    }
    
    private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        // Define valid status transitions
        var validTransitions = new Dictionary<OrderStatus, OrderStatus[]>
        {
            [OrderStatus.Pending] = new[] { OrderStatus.Confirmed, OrderStatus.Cancelled },
            [OrderStatus.Confirmed] = new[] { OrderStatus.Processing, OrderStatus.Cancelled },
            [OrderStatus.Processing] = new[] { OrderStatus.Shipped, OrderStatus.Cancelled },
            [OrderStatus.Shipped] = new[] { OrderStatus.Delivered, OrderStatus.Cancelled },
            [OrderStatus.Delivered] = new[] { OrderStatus.Refunded },
            [OrderStatus.Cancelled] = new OrderStatus[0], // Terminal state
            [OrderStatus.Refunded] = new OrderStatus[0]  // Terminal state
        };

        if (!validTransitions.ContainsKey(currentStatus))
            return false;

        return validTransitions[currentStatus].Contains(newStatus);
    }
    
    private async Task<OrderDetailDto> MapToOrderDetailDtoAsync(Order order)
    {
        var customer = await _customerRepository.GetByIdAsync(order.CustomerId);
        var store = await _storeRepository.GetByIdAsync(order.StoreId);
        var orderItems = await _orderItemRepository.GetByOrderAsync(order.Id);
        
        var itemDtos = await MapToOrderItemDtosAsync(orderItems);
        
        return new OrderDetailDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerId = order.CustomerId,
            CustomerName = customer != null ? customer.User.FullName ?? "Unknown" : "Unknown",
            StoreId = order.StoreId,
            StoreName = store?.Name ?? "Unknown",
            Status = order.Status,
            SubTotal = order.Subtotal,
            TaxAmount = order.TaxAmount,
            ShippingAmount = order.ShippingAmount,
            DiscountAmount = order.DiscountAmount,
            TotalAmount = order.TotalAmount,
            Currency = order.Currency,
            Notes = order.Notes,
            // These properties are not available in Order entity
            CreatedAt = order.CreatedAt,
            ModifiedAt = order.ModifiedAt,
            ShippedAt = order.ShippedAt,
            DeliveredAt = order.DeliveredAt,
            CancelledAt = order.CancelledAt,
            Items = itemDtos
        };
    }
    
    private async Task<OrderDto> MapToOrderDtoAsync(Order order)
    {
        var customer = await _customerRepository.GetByIdAsync(order.CustomerId);
        var store = await _storeRepository.GetByIdAsync(order.StoreId);
        var orderItems = await _orderItemRepository.GetByOrderAsync(order.Id);
        
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerId = order.CustomerId,
            CustomerName = customer != null ? customer.User.FullName ?? "Unknown" : "Unknown",
            StoreId = order.StoreId,
            StoreName = store?.Name ?? "Unknown",
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            Currency = order.Currency,
            ItemCount = orderItems.Count(),
            CreatedAt = order.CreatedAt,
            ShippedAt = order.ShippedAt,
            DeliveredAt = order.DeliveredAt
        };
    }
    
    private async Task<IEnumerable<OrderDto>> MapToOrderListDtosAsync(IEnumerable<Order> orders)
    {
        var orderDtos = new List<OrderDto>();
        
        foreach (var order in orders)
        {
            orderDtos.Add(await MapToOrderDtoAsync(order));
        }
        
        return orderDtos;
    }
    
    private async Task<OrderItemDto> MapToOrderItemDtoAsync(OrderItem item)
    {
        var product = await _productRepository.GetByIdAsync(item.ProductId);
        
        return new OrderItemDto
        {
            Id = item.Id,
            OrderId = item.OrderId,
            ProductId = item.ProductId,
            ProductName = product?.Name ?? "Unknown Product",
            ProductSku = product?.Sku ?? "Unknown",
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.TotalPrice,
            // Currency and Notes are not available in OrderItem entity
        };
    }
    
    private async Task<IEnumerable<OrderItemDto>> MapToOrderItemDtosAsync(IEnumerable<OrderItem> items)
    {
        var itemDtos = new List<OrderItemDto>();
        
        foreach (var item in items)
        {
            itemDtos.Add(await MapToOrderItemDtoAsync(item));
        }
        
        return itemDtos;
    }
    
    #endregion

    #region Customer API Methods

    public async Task<PaginatedResult<CustomerOrderDto>> GetOrdersByUserIdAsync(long userId, int page = 1, int pageSize = 10)
    {
        try
        {
            var orders = await _orderRepository.GetByCustomerAsync(userId);
            var totalCount = orders.Count();
            
            var paginatedOrders = orders
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var customerOrderDtos = new List<CustomerOrderDto>();
            
            foreach (var order in paginatedOrders)
            {
                var orderItems = await _orderItemRepository.GetByOrderAsync(order.Id);
                var itemDtos = new List<CustomerOrderItemDto>();
                
                foreach (var item in orderItems)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    itemDtos.Add(new CustomerOrderItemDto
                    {
                        ProductId = item.ProductId,
                        ProductName = product?.Name ?? "Unknown Product",
                        ProductImage =  "/images/default-product.jpg",
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    });
                }

                var customer = await _customerRepository.GetByIdAsync(order.CustomerId);
                customerOrderDtos.Add(new CustomerOrderDto
                {
                    Id = order.Id,
                    OrderNumber = order.OrderNumber,
                    Status = order.Status.ToString(),
                    TotalAmount = order.TotalAmount,
                    CreatedAt = order.CreatedAt,
                    CustomerName = customer?.User?.FullName ?? "Unknown",
                    CustomerEmail = customer?.User?.Email ?? "Unknown",
                    Items = itemDtos
                });
            }

            return new PaginatedResult<CustomerOrderDto>
            {
                Items = customerOrderDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders for user ID: {UserId}", userId);
            throw new RepositoryException("Sipariş bilgileri alınırken bir hata oluştu", "GetOrdersByUserId", "Order", ex);
        }
    }

    public async Task<PaginatedResult<CustomerWishlistItemDto>> GetWishlistItemsByUserIdAsync(long userId, int page = 1, int pageSize = 10)
    {
        try
        {
            // TODO: Wishlist tablosu oluşturulduğunda bu kısım güncellenecek
            // Şu anda mock data döndürüyoruz
            var mockWishlistItems = new List<CustomerWishlistItemDto>
            {
                new CustomerWishlistItemDto
                {
                    Id = 1,
                    ProductId = 1,
                    ProductName = "iPhone 15 Pro",
                    ProductImage = "/images/iphone15pro.jpg",
                    Price = 299.99m,
                    IsInStock = true,
                    StockQuantity = 15,
                    AddedAt = DateTime.UtcNow.AddDays(-10)
                },
                new CustomerWishlistItemDto
                {
                    Id = 2,
                    ProductId = 2,
                    ProductName = "AirPods Pro",
                    ProductImage = "/images/airpodspro.jpg",
                    Price = 149.99m,
                    IsInStock = true,
                    StockQuantity = 25,
                    AddedAt = DateTime.UtcNow.AddDays(-5)
                }
            };

            var totalCount = mockWishlistItems.Count;
            var paginatedItems = mockWishlistItems
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedResult<CustomerWishlistItemDto>
            {
                Items = paginatedItems,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting wishlist items for user ID: {UserId}", userId);
            throw new RepositoryException("Favori ürünler alınırken bir hata oluştu", "GetWishlistItemsByUserId", "Wishlist", ex);
        }
    }

    #endregion
}
