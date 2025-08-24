using FluentValidation;
using Application.DTOs.Products;

namespace Api.Validators;

/// <summary>
/// Ürün arama isteği için validasyon kuralları
/// </summary>
public sealed class ProductSearchRequestValidator : AbstractValidator<ProductSearchRequest>
{
    public ProductSearchRequestValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(200).WithMessage("Arama terimi en fazla 200 karakter olabilir")
            .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm));

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

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum fiyat 0'dan küçük olamaz")
            .LessThanOrEqualTo(999999.99m).WithMessage("Minimum fiyat 999,999.99'dan küçük olmalıdır")
            .When(x => x.MinPrice.HasValue);

        RuleFor(x => x.MaxPrice)
            .GreaterThan(0).WithMessage("Maksimum fiyat 0'dan büyük olmalıdır")
            .LessThanOrEqualTo(999999.99m).WithMessage("Maksimum fiyat 999,999.99'dan küçük olmalıdır")
            .When(x => x.MaxPrice.HasValue);

        RuleFor(x => x.MinPrice)
            .Must((request, minPrice) => !minPrice.HasValue || !request.MaxPrice.HasValue || minPrice <= request.MaxPrice)
            .WithMessage("Minimum fiyat maksimum fiyattan küçük veya eşit olmalıdır");

        RuleFor(x => x.SortBy)
            .Must(BeValidSortBy).WithMessage("Geçersiz sıralama alanı. Kullanılabilir: Relevance, Name, Price, CreatedAt, StockQty")
            .When(x => !string.IsNullOrWhiteSpace(x.SortBy));

        RuleFor(x => x.SortOrder)
            .Must(BeValidSortOrder).WithMessage("Geçersiz sıralama yönü. Kullanılabilir: Asc, Desc")
            .When(x => !string.IsNullOrWhiteSpace(x.SortOrder));

        RuleFor(x => x.Brand)
            .MaximumLength(100).WithMessage("Marka adı en fazla 100 karakter olabilir")
            .When(x => !string.IsNullOrWhiteSpace(x.Brand));

        RuleFor(x => x.Tags)
            .MaximumLength(500).WithMessage("Etiketler en fazla 500 karakter olabilir")
            .When(x => !string.IsNullOrWhiteSpace(x.Tags));
    }

    private static bool BeValidSortBy(string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return true;
        
        var validSortBy = new[] { "Relevance", "Name", "Price", "CreatedAt", "StockQty", "DisplayOrder" };
        return validSortBy.Contains(sortBy, StringComparer.OrdinalIgnoreCase);
    }

    private static bool BeValidSortOrder(string? sortOrder)
    {
        if (string.IsNullOrWhiteSpace(sortOrder)) return true;
        
        var validSortOrder = new[] { "Asc", "Desc" };
        return validSortOrder.Contains(sortOrder, StringComparer.OrdinalIgnoreCase);
    }
}
