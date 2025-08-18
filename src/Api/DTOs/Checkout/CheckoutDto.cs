namespace Api.DTOs.Checkout;

public class CheckoutSessionDto
{
    public string SessionId { get; set; } = string.Empty;
    public string PaytrToken { get; set; } = string.Empty;
    public string PaytrIframeUrl { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public CheckoutSummaryDto Summary { get; set; } = new();
}

public class CheckoutSummaryDto
{
    public long OrderId { get; set; }
    public string MerchantOid { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal ShippingTotal { get; set; }
    public decimal Total { get; set; }
    public string Currency { get; set; } = "TRY";
    
    // Customer information
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ShipAddressJson { get; set; } = string.Empty;
    
    // Store groups for multi-vendor
    public List<CheckoutStoreGroupDto> StoreGroups { get; set; } = new();
}

public class CheckoutStoreGroupDto
{
    public long StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string StoreSlug { get; set; } = string.Empty;
    public long SellerId { get; set; }
    public decimal ItemsTotal { get; set; }
    public decimal ShippingTotal { get; set; }
    public decimal GroupTotal { get; set; }
    public List<CheckoutItemDto> Items { get; set; } = new();
}

public class CheckoutItemDto
{
    public long ProductId { get; set; }
    public long? VariantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}

public class CreateCheckoutDto
{
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ShipAddressJson { get; set; } = string.Empty;
    public List<long> CartItemIds { get; set; } = new();
}
