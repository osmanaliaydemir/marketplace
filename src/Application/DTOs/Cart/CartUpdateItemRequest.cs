using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Cart;

public sealed record CartUpdateItemRequest
{
    [Required(ErrorMessage = "Miktar zorunludur")]
    [Range(1, 100, ErrorMessage = "Miktar 1-100 arasında olmalıdır")]
    public int Quantity { get; init; }
    
    [StringLength(500, ErrorMessage = "Not en fazla 500 karakter olabilir")]
    public string? Note { get; init; }
}
