namespace Api.DTOs.Cart;

public class CartItemDto
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public long? VariantId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSlug { get; set; } = string.Empty;
    public string? ProductImage { get; set; }
    public decimal UnitPrice { get; set; }
    public int Qty { get; set; }
    public decimal Total { get; set; }
    public string Currency { get; set; } = "TRY";
    
    // Store information
    public long StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string StoreSlug { get; set; } = string.Empty;
}

public class CartSummaryDto
{
    public List<CartItemDto> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal ShippingTotal { get; set; }
    public decimal Total { get; set; }
    public string Currency { get; set; } = "TRY";
    public int TotalItems => Items.Sum(x => x.Qty);
    
    // Grouped by store for multi-vendor cart
    public List<StoreCartGroupDto> StoreGroups { get; set; } = new();
}

public class StoreCartGroupDto
{
    public long StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string StoreSlug { get; set; } = string.Empty;
    public List<CartItemDto> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal ShippingTotal { get; set; }
    public decimal Total { get; set; }
}

public class AddToCartDto
{
    public long ProductId { get; set; }
    public long? VariantId { get; set; }
    public int Qty { get; set; } = 1;
}

public class UpdateCartItemDto
{
    public int Qty { get; set; }
}
