using FluentValidation;
using Api.DTOs.Products;

namespace Api.Validators;

public sealed class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kategori adı gereklidir")
            .MaximumLength(100).WithMessage("Kategori adı en fazla 100 karakter olabilir")
            .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s\-]+$").WithMessage("Kategori adı sadece harf, boşluk ve tire içerebilir");

        RuleFor(x => x.Slug)
            .MaximumLength(150).WithMessage("Slug en fazla 150 karakter olabilir")
            .Matches(@"^[a-z0-9\-]+$").WithMessage("Slug sadece küçük harf, rakam ve tire içerebilir")
            .When(x => !string.IsNullOrWhiteSpace(x.Slug));

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Resim URL'si en fazla 500 karakter olabilir")
            .Must(BeValidUrl).WithMessage("Geçerli bir URL giriniz")
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));

        RuleFor(x => x.IconClass)
            .MaximumLength(50).WithMessage("İkon sınıfı en fazla 50 karakter olabilir")
            .When(x => !string.IsNullOrWhiteSpace(x.IconClass));

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Görüntüleme sırası 0 veya daha büyük olmalıdır")
            .LessThanOrEqualTo(999).WithMessage("Görüntüleme sırası 999'dan küçük olmalıdır");

        RuleFor(x => x.MetaTitle)
            .MaximumLength(60).WithMessage("Meta başlık en fazla 60 karakter olabilir")
            .When(x => !string.IsNullOrWhiteSpace(x.MetaTitle));

        RuleFor(x => x.MetaDescription)
            .MaximumLength(160).WithMessage("Meta açıklama en fazla 160 karakter olabilir")
            .When(x => !string.IsNullOrWhiteSpace(x.MetaDescription));

        RuleFor(x => x.ParentId)
            .GreaterThan(0).WithMessage("Geçersiz üst kategori ID'si")
            .When(x => x.ParentId.HasValue);
    }

    private static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
