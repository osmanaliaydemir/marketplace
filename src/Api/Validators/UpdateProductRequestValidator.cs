using FluentValidation;
using Application.DTOs.Products;

namespace Api.Validators;

/// <summary>
/// Ürün güncelleme isteği için validasyon kuralları
/// </summary>
public sealed class UpdateProductRequestValidator : AbstractValidator<ProductUpdateRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ürün adı gereklidir")
            .MaximumLength(255).WithMessage("Ürün adı en fazla 255 karakter olabilir")
            .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ0-9\s\-\.\(\)]+$")
            .WithMessage("Ürün adı sadece harf, rakam, boşluk, tire, nokta ve parantez içerebilir");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Ürün açıklaması gereklidir")
            .MinimumLength(10).WithMessage("Ürün açıklaması en az 10 karakter olmalıdır")
            .MaximumLength(5000).WithMessage("Ürün açıklaması en fazla 5000 karakter olabilir");

        RuleFor(x => x.ShortDescription)
            .MaximumLength(500).WithMessage("Kısa açıklama en fazla 500 karakter olabilir")
            .When(x => !string.IsNullOrWhiteSpace(x.ShortDescription));

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Geçerli bir kategori seçilmelidir");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır")
            .LessThanOrEqualTo(999999.99m).WithMessage("Fiyat 999,999.99'dan küçük olmalıdır");

        RuleFor(x => x.CompareAtPrice)
            .GreaterThan(0).WithMessage("Karşılaştırma fiyatı 0'dan büyük olmalıdır")
            .LessThanOrEqualTo(999999.99m).WithMessage("Karşılaştırma fiyatı 999,999.99'dan küçük olmalıdır")
            .When(x => x.CompareAtPrice.HasValue);

        RuleFor(x => x.CompareAtPrice)
            .Must((product, comparePrice) => !comparePrice.HasValue || comparePrice > product.Price)
            .WithMessage("Karşılaştırma fiyatı normal fiyattan büyük olmalıdır");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Para birimi gereklidir")
            .Length(3).WithMessage("Para birimi 3 karakter olmalıdır")
            .Matches(@"^[A-Z]{3}$").WithMessage("Para birimi 3 büyük harf olmalıdır (örn: TRY, USD, EUR)");

        RuleFor(x => x.StockQty)
            .GreaterThanOrEqualTo(0).WithMessage("Stok miktarı 0 veya daha büyük olmalıdır")
            .LessThanOrEqualTo(999999).WithMessage("Stok miktarı 999,999'dan küçük olmalıdır");

        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(0).WithMessage("Ağırlık 0 veya daha büyük olmalıdır")
            .LessThanOrEqualTo(999999.99m).WithMessage("Ağırlık 999,999.99'dan küçük olmalıdır");

        RuleFor(x => x.MinOrderQty)
            .GreaterThan(0).WithMessage("Minimum sipariş miktarı 0'dan büyük olmalıdır")
            .LessThanOrEqualTo(9999).WithMessage("Minimum sipariş miktarı 9,999'dan küçük olmalıdır")
            .When(x => x.MinOrderQty.HasValue);

        RuleFor(x => x.MaxOrderQty)
            .GreaterThan(0).WithMessage("Maksimum sipariş miktarı 0'dan büyük olmalıdır")
            .LessThanOrEqualTo(9999).WithMessage("Maksimum sipariş miktarı 9,999'dan küçük olmalıdır")
            .When(x => x.MaxOrderQty.HasValue);

        RuleFor(x => x.MinOrderQty)
            .Must((product, minQty) => !minQty.HasValue || !product.MaxOrderQty.HasValue || minQty <= product.MaxOrderQty)
            .WithMessage("Minimum sipariş miktarı maksimum sipariş miktarından küçük veya eşit olmalıdır");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Görüntüleme sırası 0 veya daha büyük olmalıdır")
            .LessThanOrEqualTo(999).WithMessage("Görüntüleme sırası 999'dan küçük olmalıdır");

        RuleFor(x => x.MetaTitle)
            .MaximumLength(255).WithMessage("Meta başlık en fazla 255 karakter olabilir")
            .When(x => !string.IsNullOrWhiteSpace(x.MetaTitle));

        RuleFor(x => x.MetaDescription)
            .MaximumLength(500).WithMessage("Meta açıklama en fazla 500 karakter olabilir")
            .When(x => !string.IsNullOrWhiteSpace(x.MetaDescription));

        RuleFor(x => x.MetaKeywords)
            .MaximumLength(500).WithMessage("Meta anahtar kelimeler en fazla 500 karakter olabilir")
            .When(x => !string.IsNullOrWhiteSpace(x.MetaKeywords));
    }
}
