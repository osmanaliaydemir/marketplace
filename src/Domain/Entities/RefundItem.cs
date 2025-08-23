namespace Domain.Entities;

public sealed class RefundItem : Domain.Models.BaseEntity
{
    public long RefundId { get; set; }
    public long OrderItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Reason { get; set; }
    
    // Navigation properties
    public Refund Refund { get; set; } = null!;
    public OrderItem OrderItem { get; set; } = null!;
}


