using FluentValidation;
using Api.DTOs.Products;

namespace Api.Validators;

/// <summary>
/// Ürün varyantı oluşturma isteği için validasyon kuralları
/// </summary>
public sealed class CreateProductVariantRequestValidator : AbstractValidator<CreateProductVariantRequest>
{
	public CreateProductVariantRequestValidator()
	{
		RuleFor(x => x.Sku)
			.MaximumLength(50).WithMessage("SKU en fazla 50 karakter olabilir")
			.When(x => !string.IsNullOrWhiteSpace(x.Sku));

		RuleFor(x => x.Barcode)
			.MaximumLength(50).WithMessage("Barkod en fazla 50 karakter olabilir")
			.When(x => !string.IsNullOrWhiteSpace(x.Barcode));

		RuleFor(x => x.VariantName)
			.MaximumLength(100).WithMessage("Varyant adı en fazla 100 karakter olabilir")
			.When(x => !string.IsNullOrWhiteSpace(x.VariantName));

		RuleFor(x => x.Price)
			.GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır")
			.LessThanOrEqualTo(999999.99m).WithMessage("Fiyat 999,999.99'dan küçük olmalıdır");

		RuleFor(x => x.CompareAtPrice)
			.GreaterThan(0).WithMessage("Karşılaştırma fiyatı 0'dan büyük olmalıdır")
			.LessThanOrEqualTo(999999.99m).WithMessage("Karşılaştırma fiyatı 999,999.99'dan küçük olmalıdır")
			.When(x => x.CompareAtPrice.HasValue);

		RuleFor(x => x.CompareAtPrice)
			.Must((variant, comparePrice) => !comparePrice.HasValue || comparePrice > variant.Price)
			.WithMessage("Karşılaştırma fiyatı normal fiyattan büyük olmalıdır");

		RuleFor(x => x.StockQty)
			.GreaterThanOrEqualTo(0).WithMessage("Stok miktarı 0 veya daha büyük olmalıdır")
			.LessThanOrEqualTo(999999).WithMessage("Stok miktarı 999,999'dan küçük olmalıdır");

		RuleFor(x => x.MinOrderQty)
			.GreaterThan(0).WithMessage("Minimum sipariş miktarı 0'dan büyük olmalıdır")
			.LessThanOrEqualTo(9999).WithMessage("Minimum sipariş miktarı 9,999'dan küçük olmalıdır")
			.When(x => x.MinOrderQty.HasValue);

		RuleFor(x => x.MaxOrderQty)
			.GreaterThan(0).WithMessage("Maksimum sipariş miktarı 0'dan büyük olmalıdır")
			.LessThanOrEqualTo(9999).WithMessage("Maksimum sipariş miktarı 9,999'dan küçük olmalıdır")
			.When(x => x.MaxOrderQty.HasValue);

		RuleFor(x => x.MinOrderQty)
			.Must((variant, minQty) => !minQty.HasValue || !variant.MaxOrderQty.HasValue || minQty <= variant.MaxOrderQty)
			.WithMessage("Minimum sipariş miktarı maksimum sipariş miktarından küçük veya eşit olmalıdır");

		RuleFor(x => x.Weight)
			.GreaterThanOrEqualTo(0).WithMessage("Ağırlık 0 veya daha büyük olmalıdır")
			.LessThanOrEqualTo(999999).WithMessage("Ağırlık 999,999 gramdan küçük olmalıdır");

		RuleFor(x => x.IsDefault)
			.NotNull().WithMessage("Varsayılan varyant bilgisi gereklidir");
	}
}
