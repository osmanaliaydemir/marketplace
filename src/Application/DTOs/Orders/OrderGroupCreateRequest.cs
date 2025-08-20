using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Orders;

public sealed record OrderGroupCreateRequest
{
    [Required(ErrorMessage = "Müşteri ID zorunludur")]
    public long CustomerId { get; init; }
    
    [Required(ErrorMessage = "Sipariş ID'leri zorunludur")]
    [MinLength(1, ErrorMessage = "En az bir sipariş seçilmelidir")]
    public IEnumerable<long> OrderIds { get; init; } = Enumerable.Empty<long>();
    
    [StringLength(500, ErrorMessage = "Not en fazla 500 karakter olabilir")]
    public string? Note { get; init; }
    
    public string? Notes { get; init; }
    public string? GroupName { get; init; }
    public bool AutoCalculateTotals { get; init; } = true;
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; } = "TRY";
}
