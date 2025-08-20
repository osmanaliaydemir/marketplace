using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Inventory;

public sealed record StockReservationRequest
{
    [Required(ErrorMessage = "Ürün ID zorunludur")]
    public long ProductId { get; init; }
    
    [Required(ErrorMessage = "Miktar zorunludur")]
    [Range(1, int.MaxValue, ErrorMessage = "Miktar 1'den büyük olmalıdır")]
    public int Quantity { get; init; }
    
    [Required(ErrorMessage = "Rezervasyon nedeni zorunludur")]
    [StringLength(500, ErrorMessage = "Rezervasyon nedeni en fazla 500 karakter olabilir")]
    public string Reason { get; init; } = string.Empty;
    
    [StringLength(100, ErrorMessage = "Referans en fazla 100 karakter olabilir")]
    public string? Reference { get; init; }
    
    public long? OrderId { get; init; }
    public long? CustomerId { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public TimeSpan? Duration { get; init; } = TimeSpan.FromHours(24);
    public Dictionary<string, string>? Metadata { get; init; }
}
