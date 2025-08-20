using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Inventory;

public sealed record StockOperationRequest
{
    [Required(ErrorMessage = "Ürün ID zorunludur")]
    public long ProductId { get; init; }
    
    [Required(ErrorMessage = "İşlem türü zorunludur")]
    public StockOperationType OperationType { get; init; }
    
    [Required(ErrorMessage = "Miktar zorunludur")]
    public int Quantity { get; init; }
    
    [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
    public string? Reason { get; init; }
    
    [StringLength(100, ErrorMessage = "Referans en fazla 100 karakter olabilir")]
    public string? Reference { get; init; }
    
    public long? OrderId { get; init; }
    public long? UserId { get; init; }
    public long? StoreId { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
    
    // Validation
    public bool IsValid => ValidateOperation();
    
    private bool ValidateOperation()
    {
        return OperationType switch
        {
            StockOperationType.Adjustment => true, // Can be positive or negative
            StockOperationType.Addition => Quantity > 0,
            StockOperationType.Subtraction => Quantity > 0,
            StockOperationType.Reservation => Quantity > 0,
            StockOperationType.Release => Quantity > 0,
            StockOperationType.Sale => Quantity > 0,
            StockOperationType.Return => Quantity > 0,
            StockOperationType.Damage => Quantity > 0,
            StockOperationType.Expiry => Quantity > 0,
            _ => false
        };
    }
}
