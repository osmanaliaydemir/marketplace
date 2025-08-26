using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Cart;

public class CartIndexModel : PageModel
{
    public List<CartItemDto>? CartItems { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    
    public async Task OnGetAsync()
    {
        // TODO: Replace with actual API call to get cart items
        // For now, create mock cart items for demonstration
        CartItems = CreateMockCartItems();
        
        // Calculate totals
        CalculateTotals();
    }
    
    private List<CartItemDto> CreateMockCartItems()
    {
        return new List<CartItemDto>
        {
            new CartItemDto
            {
                ProductId = 1,
                ProductName = "Kablosuz Bluetooth Kulaklık",
                Sku = "SKU-001",
                VariantName = "Siyah",
                ImageUrl = "https://via.placeholder.com/150x150/0d6efd/ffffff?text=Headphones",
                Price = 299.99m,
                CompareAtPrice = 399.99m,
                Quantity = 2,
                StockQty = 50
            },
            new CartItemDto
            {
                ProductId = 2,
                ProductName = "Akıllı Saat Pro Max",
                Sku = "SKU-002",
                VariantName = "Gümüş",
                ImageUrl = "https://via.placeholder.com/150x150/198754/ffffff?text=Smartwatch",
                Price = 1299.99m,
                CompareAtPrice = 1499.99m,
                Quantity = 1,
                StockQty = 25
            },
            new CartItemDto
            {
                ProductId = 3,
                ProductName = "Laptop Stand Pro",
                Sku = "SKU-003",
                VariantName = "Alüminyum",
                ImageUrl = "https://via.placeholder.com/150x150/dc3545/ffffff?text=Laptop+Stand",
                Price = 89.99m,
                CompareAtPrice = 89.99m,
                Quantity = 1,
                StockQty = 100
            },
            new CartItemDto
            {
                ProductId = 4,
                ProductName = "Kablosuz Mouse Gaming",
                Sku = "SKU-004",
                VariantName = "RGB",
                ImageUrl = "https://via.placeholder.com/150x150/ffc107/000000?text=Gaming+Mouse",
                Price = 149.99m,
                CompareAtPrice = 199.99m,
                Quantity = 1,
                StockQty = 75
            }
        };
    }
    
    private void CalculateTotals()
    {
        if (CartItems == null || !CartItems.Any())
        {
            Subtotal = 0;
            ShippingCost = 0;
            TaxAmount = 0;
            Total = 0;
            return;
        }
        
        // Calculate subtotal
        Subtotal = CartItems.Sum(item => item.Price * item.Quantity);
        
        // Calculate shipping cost (free shipping over 150₺)
        ShippingCost = Subtotal >= 150 ? 0 : 29.99m;
        
        // Calculate tax (18% KDV)
        TaxAmount = Subtotal * 0.18m;
        
        // Calculate total
        Total = Subtotal + ShippingCost + TaxAmount;
    }
}

// Cart Item DTO
public class CartItemDto
{
    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? VariantName { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal CompareAtPrice { get; set; }
    public int Quantity { get; set; }
    public int StockQty { get; set; }
}
