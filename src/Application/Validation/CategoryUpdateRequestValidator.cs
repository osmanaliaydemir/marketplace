using Application.DTOs.Categories;
using FluentValidation;

namespace Application.Validation;

public sealed class CategoryUpdateRequestValidator : AbstractValidator<CategoryUpdateRequest>
{
    public CategoryUpdateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kategori adı boş olamaz")
            .MaximumLength(255).WithMessage("Kategori adı en fazla 255 karakter olabilir")
            .MinimumLength(2).WithMessage("Kategori adı en az 2 karakter olmalıdır");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ParentId)
            .GreaterThan(0).WithMessage("Geçerli bir üst kategori seçilmelidir")
            .When(x => x.ParentId.HasValue);

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Resim URL en fazla 500 karakter olabilir")
            .Must(BeValidUrl).WithMessage("Geçerli bir resim URL'i giriniz")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        RuleFor(x => x.IconClass)
            .MaximumLength(100).WithMessage("İkon sınıfı en fazla 100 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.IconClass));

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Görünüm sırası 0'dan küçük olamaz");

        RuleFor(x => x.MetaTitle)
            .MaximumLength(255).WithMessage("Meta başlık en fazla 255 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.MetaTitle));

        RuleFor(x => x.MetaDescription)
            .MaximumLength(500).WithMessage("Meta açıklama en fazla 500 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.MetaDescription));

        // Business Rules
        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Description) || !string.IsNullOrEmpty(x.ImageUrl))
            .WithMessage("Kategori için açıklama veya resim eklenmelidir");

        RuleFor(x => x)
            .Must(x => x.ParentId == null || x.ParentId != 0)
            .WithMessage("Üst kategori ID 0 olamaz");
    }

    private static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
