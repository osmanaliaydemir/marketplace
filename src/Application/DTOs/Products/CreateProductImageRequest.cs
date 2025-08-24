using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Products;

public sealed record CreateProductImageRequest
{
    [Required(ErrorMessage = "Resim URL'si zorunludur")]
    [StringLength(500, ErrorMessage = "Resim URL'si en fazla 500 karakter olabilir")]
    public string ImageUrl { get; init; } = string.Empty;
    
    [StringLength(255, ErrorMessage = "Alt metin en fazla 255 karakter olabilir")]
    public string? AltText { get; init; }
    
    [StringLength(500, ErrorMessage = "Başlık en fazla 500 karakter olabilir")]
    public string? Title { get; init; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Görüntüleme sırası 0'dan küçük olamaz")]
    public int DisplayOrder { get; init; } = 0;
    
    public bool IsPrimary { get; init; } = false;
    public bool IsActive { get; init; } = true;
}
