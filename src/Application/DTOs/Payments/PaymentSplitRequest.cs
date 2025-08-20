using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Payments;

public sealed record PaymentSplitRequest
{
    public long StoreId { get; init; }
    public decimal CommissionRate { get; init; }
}

public sealed record PaymentSplitDetailDto
{
    [Required(ErrorMessage = "Mağaza ID zorunludur")]
    public long StoreId { get; init; }
    
    [Required(ErrorMessage = "Tutar zorunludur")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Tutar 0'dan büyük olmalıdır")]
    public decimal Amount { get; init; }
    
    [StringLength(100, ErrorMessage = "Referans en fazla 100 karakter olabilir")]
    public string? Reference { get; init; }
}
