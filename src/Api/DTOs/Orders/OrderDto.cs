namespace Api.DTOs.Orders;

public class OrderListDto
{
    public long Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Currency { get; set; } = "TRY";
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int StoreCount { get; set; }
    public int TotalItems { get; set; }
}

public class OrderDetailDto
{
    public long Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public long? BuyerId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ShipAddressJson { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Currency { get; set; } = "TRY";
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public List<OrderGroupDetailDto> StoreGroups { get; set; } = new();
    public PaymentInfoDto? Payment { get; set; }
}

public class OrderGroupDetailDto
{
    public long Id { get; set; }
    public long StoreId { get; set; }
    public long SellerId { get; set; }
    public decimal ItemsTotal { get; set; }
    public decimal ShippingTotal { get; set; }
    public decimal GroupTotal { get; set; }
    public string Status { get; set; } = string.Empty;
    
    // Navigation properties
    public StoreInfoDto Store { get; set; } = new();
    public SellerInfoDto Seller { get; set; } = new();
    public List<OrderItemDetailDto> Items { get; set; } = new();
    public ShipmentInfoDto? Shipment { get; set; }
}

public class OrderItemDetailDto
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public long? VariantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
    
    // Product info
    public string ProductSlug { get; set; } = string.Empty;
    public string? ProductImage { get; set; }
}

public class PaymentInfoDto
{
    public long Id { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string? ProviderTx { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Gross { get; set; }
    public decimal FeePlatform { get; set; }
    public decimal FeePsp { get; set; }
    public decimal NetToSellers { get; set; }
    public string MerchantOid { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? CapturedAt { get; set; }
}

public class ShipmentInfoDto
{
    public long Id { get; set; }
    public string Carrier { get; set; } = string.Empty;
    public string? Service { get; set; }
    public string? TrackingNumber { get; set; }
    public string? LabelUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
}

// Seller-specific order views
public class SellerOrderListDto
{
    public long Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal GroupTotal { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public int ItemCount { get; set; }
}

// Helper DTOs (reused from other namespaces)
public class StoreInfoDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
}

public class SellerInfoDto
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal CommissionRate { get; set; }
    public bool IsActive { get; set; }
}
