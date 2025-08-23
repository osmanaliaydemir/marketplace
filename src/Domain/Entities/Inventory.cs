namespace Domain.Entities;

public sealed class Inventory : Domain.Models.BaseEntity
{
    public long ProductId { get; set; }
    public long StoreId { get; set; }
    public int StockQty { get; set; } // Ana stok miktarı (CurrentStock yerine)
    public int ReservedQty { get; set; } // Reserve edilen miktar (ReservedStock yerine)
    public int AvailableStock { get; set; } // Hesaplanan alan (StockQty - ReservedQty)
    public int MinStockLevel { get; set; }
    public int MaxStockLevel { get; set; }
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow; // Eksik property eklendi
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public Product Product { get; set; } = null!;
    public Store Store { get; set; } = null!;
}


