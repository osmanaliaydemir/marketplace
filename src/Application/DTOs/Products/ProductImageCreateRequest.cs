using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Products;

public sealed record ProductImageCreateRequest
{
    [Required(ErrorMessage = "Resim URL'i zorunludur")]
    [Url(ErrorMessage = "Geçerli bir resim URL'i giriniz")]
    [StringLength(500, ErrorMessage = "Resim URL en fazla 500 karakter olabilir")]
    public string ImageUrl { get; init; } = string.Empty;
    
    [StringLength(255, ErrorMessage = "Alt metin en fazla 255 karakter olabilir")]
    public string? AltText { get; init; }
    
    [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
    public string? Description { get; init; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Sıra numarası 1'den büyük olmalıdır")]
    public int DisplayOrder { get; init; } = 1;
    
    public bool IsPrimary { get; init; } = false;
    public bool IsActive { get; init; } = true;
}
