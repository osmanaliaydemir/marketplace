using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Cart;

public sealed record CartAddItemRequest
{
    [Required(ErrorMessage = "Ürün ID zorunludur")]
    public long ProductId { get; init; }
    
    [Required(ErrorMessage = "Miktar zorunludur")]
    [Range(1, int.MaxValue, ErrorMessage = "Miktar 1'den küçük olamaz")]
    public int Quantity { get; init; } = 1;
    
    public long? ProductVariantId { get; init; }
    
    public long? VariantId { get; init; }
    
    [StringLength(500, ErrorMessage = "Not en fazla 500 karakter olabilir")]
    public string? Note { get; init; }
    
    public bool UpdateIfExists { get; init; } = false;
}
