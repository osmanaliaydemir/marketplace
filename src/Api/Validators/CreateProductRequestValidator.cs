using Application.DTOs.Products;
using FluentValidation;

namespace Api.Validators;

public sealed class CreateProductRequestValidator : AbstractValidator<ProductCreateRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ürün adı zorunludur")
            .MaximumLength(255).WithMessage("Ürün adı en fazla 255 karakter olabilir");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Ürün açıklaması zorunludur")
            .MaximumLength(4000).WithMessage("Ürün açıklaması en fazla 4000 karakter olabilir");

        RuleFor(x => x.ShortDescription)
            .MaximumLength(500).WithMessage("Kısa açıklama en fazla 500 karakter olabilir");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Geçerli bir kategori seçilmelidir");

        RuleFor(x => x.StoreId)
            .GreaterThan(0).WithMessage("Geçerli bir mağaza seçilmelidir");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır")
            .PrecisionScale(19, 4, false).WithMessage("Fiyat en fazla 4 ondalık basamak içerebilir");

        RuleFor(x => x.CompareAtPrice)
            .GreaterThan(0).When(x => x.CompareAtPrice.HasValue)
            .WithMessage("Karşılaştırma fiyatı 0'dan büyük olmalıdır")
            .PrecisionScale(19, 4, false).When(x => x.CompareAtPrice.HasValue)
            .WithMessage("Karşılaştırma fiyatı en fazla 4 ondalık basamak içerebilir")
            .GreaterThan(x => x.Price).When(x => x.CompareAtPrice.HasValue)
            .WithMessage("Karşılaştırma fiyatı normal fiyattan büyük olmalıdır");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Para birimi zorunludur")
            .Length(3).WithMessage("Para birimi 3 karakter olmalıdır")
            .Matches("^[A-Z]{3}$").WithMessage("Para birimi 3 büyük harf olmalıdır (örn: TRY, USD)");

        RuleFor(x => x.StockQty)
            .GreaterThanOrEqualTo(0).WithMessage("Stok miktarı 0'dan küçük olamaz");

        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(0).WithMessage("Ağırlık 0'dan küçük olamaz")
            .PrecisionScale(19, 4, false).WithMessage("Ağırlık en fazla 4 ondalık basamak içerebilir");

        RuleFor(x => x.MinOrderQty)
            .GreaterThan(0).WithMessage("Minimum sipariş miktarı 1'den küçük olamaz");

        RuleFor(x => x.MaxOrderQty)
            .GreaterThan(0).When(x => x.MaxOrderQty.HasValue)
            .WithMessage("Maksimum sipariş miktarı 0'dan büyük olmalıdır")
            .GreaterThanOrEqualTo(x => x.MinOrderQty).When(x => x.MaxOrderQty.HasValue)
            .WithMessage("Maksimum sipariş miktarı minimum miktardan büyük veya eşit olmalıdır");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Görüntüleme sırası 0'dan küçük olamaz");

        RuleFor(x => x.SellerId)
            .GreaterThan(0).WithMessage("Geçerli bir satıcı seçilmelidir");

        RuleFor(x => x.MetaTitle)
            .MaximumLength(255).When(x => !string.IsNullOrEmpty(x.MetaTitle))
            .WithMessage("Meta başlık en fazla 255 karakter olabilir");

        RuleFor(x => x.MetaDescription)
            .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.MetaDescription))
            .WithMessage("Meta açıklama en fazla 500 karakter olabilir");

        RuleFor(x => x.MetaKeywords)
            .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.MetaKeywords))
            .WithMessage("Meta anahtar kelimeler en fazla 500 karakter olabilir");
    }
}
