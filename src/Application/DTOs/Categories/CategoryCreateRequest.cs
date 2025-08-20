using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Categories;

public sealed record CategoryCreateRequest
{
    [Required(ErrorMessage = "Kategori adı zorunludur")]
    [StringLength(255, ErrorMessage = "Kategori adı en fazla 255 karakter olabilir")]
    public string Name { get; init; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
    public string? Description { get; init; }
    
    public long? ParentId { get; init; }
    
    [StringLength(500, ErrorMessage = "Resim URL en fazla 500 karakter olabilir")]
    public string? ImageUrl { get; init; }
    
    [StringLength(100, ErrorMessage = "İkon sınıfı en fazla 100 karakter olabilir")]
    public string? IconClass { get; init; }
    
    [Required(ErrorMessage = "Görünüm sırası zorunludur")]
    [Range(0, int.MaxValue, ErrorMessage = "Görünüm sırası 0'dan küçük olamaz")]
    public int DisplayOrder { get; init; } = 0;
    
    [StringLength(255, ErrorMessage = "Meta başlık en fazla 255 karakter olabilir")]
    public string? MetaTitle { get; init; }
    
    [StringLength(500, ErrorMessage = "Meta açıklama en fazla 500 karakter olabilir")]
    public string? MetaDescription { get; init; }
    
    public bool IsActive { get; init; } = true;
    public bool IsFeatured { get; init; } = false;
}
