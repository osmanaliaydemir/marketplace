using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Orders;

public record OrderListRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Sayfa numarası 1'den küçük olamaz")]
    public int Page { get; init; } = 1;
    
    [Range(1, 100, ErrorMessage = "Sayfa boyutu 1-100 arasında olmalıdır")]
    public int PageSize { get; init; } = 20;
    
    public string? Status { get; init; }
    public long? CustomerId { get; init; }
    public long? StoreId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? SortBy { get; init; } = "CreatedAt";
    public bool SortDescending { get; init; } = true;
}
