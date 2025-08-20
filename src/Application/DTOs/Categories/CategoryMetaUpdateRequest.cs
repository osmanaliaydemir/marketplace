using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Categories;

public sealed record CategoryMetaUpdateRequest
{
    [StringLength(255, ErrorMessage = "Meta başlık en fazla 255 karakter olabilir")]
    public string? MetaTitle { get; init; }
    
    [StringLength(500, ErrorMessage = "Meta açıklama en fazla 500 karakter olabilir")]
    public string? MetaDescription { get; init; }
    
    [StringLength(500, ErrorMessage = "Meta anahtar kelimeler en fazla 500 karakter olabilir")]
    public string? MetaKeywords { get; init; }
}
