using FluentValidation;
using Application.DTOs.Products;

namespace Api.Validators;

/// <summary>
/// Ürün listeleme isteği için validasyon kuralları
/// </summary>
public sealed class ProductListRequestValidator : AbstractValidator<ProductListRequest>
{
    public ProductListRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Sayfa numarası 0'dan büyük olmalıdır")
            .LessThanOrEqualTo(1000).WithMessage("Sayfa numarası 1000'den küçük olmalıdır");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Sayfa boyutu 0'dan büyük olmalıdır")
            .LessThanOrEqualTo(100).WithMessage("Sayfa boyutu 100'den küçük olmalıdır");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Geçersiz kategori ID'si")
            .When(x => x.CategoryId.HasValue);

        RuleFor(x => x.StoreId)
            .GreaterThan(0).WithMessage("Geçersiz mağaza ID'si")
            .When(x => x.StoreId.HasValue);

        RuleFor(x => x.SortBy)
            .Must(BeValidSortBy).WithMessage("Geçersiz sıralama alanı. Kullanılabilir: CreatedAt, Name, Price, StockQty, DisplayOrder")
            .When(x => !string.IsNullOrWhiteSpace(x.SortBy));

        RuleFor(x => x.SortOrder)
            .Must(BeValidSortOrder).WithMessage("Geçersiz sıralama yönü. Kullanılabilir: Asc, Desc")
            .When(x => !string.IsNullOrWhiteSpace(x.SortOrder));
    }

    private static bool BeValidSortBy(string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return true;
        
        var validSortBy = new[] { "CreatedAt", "Name", "Price", "StockQty", "DisplayOrder" };
        return validSortBy.Contains(sortBy, StringComparer.OrdinalIgnoreCase);
    }

    private static bool BeValidSortOrder(string? sortOrder)
    {
        if (string.IsNullOrWhiteSpace(sortOrder)) return true;
        
        var validSortOrder = new[] { "Asc", "Desc" };
        return validSortOrder.Contains(sortOrder, StringComparer.OrdinalIgnoreCase);
    }
}
