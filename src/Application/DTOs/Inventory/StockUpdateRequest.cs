using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Inventory;

public sealed record StockUpdateRequest
{
    [Required(ErrorMessage = "Ürün ID zorunludur")]
    public long ProductId { get; init; }
    
    [Required(ErrorMessage = "Miktar zorunludur")]
    public int Quantity { get; init; }
    
    [Required(ErrorMessage = "İşlem türü zorunludur")]
    public StockOperationType OperationType { get; init; }
    
    [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
    public string? Reason { get; init; }
    
    [StringLength(100, ErrorMessage = "Referans en fazla 100 karakter olabilir")]
    public string? Reference { get; init; }
    
    public long? OrderId { get; init; }
    public long? UserId { get; init; }
    
    // Validation
    public bool IsValid => 
        OperationType != StockOperationType.Unknown && 
        (OperationType == StockOperationType.Adjustment || Quantity > 0);
}

public enum StockOperationType
{
    Unknown = 0,
    Addition = 1,        // Stok ekleme
    Subtraction = 2,     // Stok çıkarma
    Adjustment = 3,      // Stok düzeltme
    Reservation = 4,     // Stok rezervasyonu
    Release = 5,         // Rezervasyon iptali
    Sale = 6,            // Satış
    Return = 7,          // İade
    Damage = 8,          // Hasar
    Expiry = 9           // Son kullanma tarihi
}
