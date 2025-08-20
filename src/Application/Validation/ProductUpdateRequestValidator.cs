using Application.DTOs.Products;
using FluentValidation;

namespace Application.Validation;

public sealed class ProductUpdateRequestValidator : AbstractValidator<ProductUpdateRequest>
{
    public ProductUpdateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ürün adı boş olamaz")
            .MaximumLength(255).WithMessage("Ürün adı en fazla 255 karakter olabilir")
            .MinimumLength(2).WithMessage("Ürün adı en az 2 karakter olmalıdır");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Ürün açıklaması boş olamaz")
            .MinimumLength(10).WithMessage("Ürün açıklaması en az 10 karakter olmalıdır");

        RuleFor(x => x.ShortDescription)
            .MaximumLength(500).WithMessage("Kısa açıklama en fazla 500 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.ShortDescription));

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Geçerli bir kategori seçilmelidir");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır")
            .PrecisionScale(10, 2, false).WithMessage("Fiyat en fazla 10 basamak ve 2 ondalık basamak olabilir");

        RuleFor(x => x.CompareAtPrice)
            .GreaterThan(0).WithMessage("Karşılaştırma fiyatı 0'dan büyük olmalıdır")
            .PrecisionScale(10, 2, false).WithMessage("Karşılaştırma fiyatı en fazla 10 basamak ve 2 ondalık basamak olabilir")
            .When(x => x.CompareAtPrice.HasValue);

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Para birimi boş olamaz")
            .Length(3).WithMessage("Para birimi 3 karakter olmalıdır")
            .Matches("^[A-Z]{3}$").WithMessage("Para birimi 3 büyük harf olmalıdır (örn: TRY, USD, EUR)");

        RuleFor(x => x.StockQty)
            .GreaterThanOrEqualTo(0).WithMessage("Stok miktarı 0'dan küçük olamaz");

        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(0).WithMessage("Ağırlık 0'dan küçük olamaz");

        RuleFor(x => x.MinOrderQty)
            .GreaterThan(0).WithMessage("Minimum sipariş miktarı 0'dan büyük olmalıdır");

        RuleFor(x => x.MaxOrderQty)
            .GreaterThan(0).WithMessage("Maksimum sipariş miktarı 0'dan büyük olmalıdır")
            .When(x => x.MaxOrderQty.HasValue);

        RuleFor(x => x.MaxOrderQty)
            .GreaterThanOrEqualTo(x => x.MinOrderQty)
            .WithMessage("Maksimum sipariş miktarı minimum sipariş miktarından küçük olamaz")
            .When(x => x.MaxOrderQty.HasValue);

        RuleFor(x => x.MetaTitle)
            .MaximumLength(255).WithMessage("Meta başlık en fazla 255 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.MetaTitle));

        RuleFor(x => x.MetaDescription)
            .MaximumLength(500).WithMessage("Meta açıklama en fazla 500 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.MetaDescription));

        RuleFor(x => x.MetaKeywords)
            .MaximumLength(500).WithMessage("Meta anahtar kelimeler en fazla 500 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.MetaKeywords));

        // Business Rules
        RuleFor(x => x)
            .Must(x => !x.CompareAtPrice.HasValue || x.CompareAtPrice > x.Price)
            .WithMessage("Karşılaştırma fiyatı normal fiyattan büyük olmalıdır");

        RuleFor(x => x)
            .Must(x => x.MaxOrderQty == null || x.MaxOrderQty >= x.MinOrderQty)
            .WithMessage("Maksimum sipariş miktarı minimum sipariş miktarından küçük olamaz");
    }
}
