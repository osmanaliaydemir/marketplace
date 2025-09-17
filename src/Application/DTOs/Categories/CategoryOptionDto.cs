namespace Application.DTOs.Categories;

/// <summary>
/// Dropdown ve select listeler için basit kategori DTO'su
/// </summary>
public sealed record CategoryOptionDto
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public long? ParentId { get; init; }
    public bool IsMainCategory { get; init; }
}
