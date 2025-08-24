using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Products;

public sealed record UpdateProductImageOrderRequest
{
    [Required(ErrorMessage = "Resim ID'si zorunludur")]
    public long ImageId { get; init; }
    
    [Required(ErrorMessage = "Yeni görüntüleme sırası zorunludur")]
    [Range(0, int.MaxValue, ErrorMessage = "Görüntüleme sırası 0'dan küçük olamaz")]
    public int NewDisplayOrder { get; init; }
}
