using FluentValidation;
using Application.DTOs.Categories;

namespace Api.Validators;

public sealed class CategorySearchRequestValidator : AbstractValidator<CategorySearchRequest>
{
    public CategorySearchRequestValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(100).WithMessage("Arama terimi en fazla 100 karakter olabilir")
            .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm));

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Sayfa numarası 1'den büyük olmalıdır")
            .LessThanOrEqualTo(1000).WithMessage("Sayfa numarası 1000'den küçük olmalıdır");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Sayfa boyutu 1'den büyük olmalıdır")
            .LessThanOrEqualTo(200).WithMessage("Sayfa boyutu 200'den küçük olmalıdır");

        RuleFor(x => x.SortBy)
            .Must(BeValidSortBy).WithMessage("Geçersiz sıralama alanı. Geçerli değerler: Name, DisplayOrder, CreatedAt")
            .When(x => !string.IsNullOrWhiteSpace(x.SortBy));

        RuleFor(x => x.SortOrder)
            .Must(BeValidSortOrder).WithMessage("Geçersiz sıralama yönü. Geçerli değerler: Asc, Desc")
            .When(x => !string.IsNullOrWhiteSpace(x.SortOrder));

        RuleFor(x => x.ParentId)
            .GreaterThan(0).WithMessage("Geçersiz üst kategori ID'si")
            .When(x => x.ParentId.HasValue);
    }

    private static bool BeValidSortBy(string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return true;
        var validSortBy = new[] { "Name", "DisplayOrder", "CreatedAt" };
        return validSortBy.Contains(sortBy, StringComparer.OrdinalIgnoreCase);
    }

    private static bool BeValidSortOrder(string? sortOrder)
    {
        if (string.IsNullOrWhiteSpace(sortOrder)) return true;
        var validSortOrder = new[] { "Asc", "Desc" };
        return validSortOrder.Contains(sortOrder, StringComparer.OrdinalIgnoreCase);
    }
}
