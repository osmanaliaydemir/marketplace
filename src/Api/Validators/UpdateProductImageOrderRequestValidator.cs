using FluentValidation;
using Application.DTOs.Products;

namespace Api.Validators;

/// <summary>
/// Ürün resim sıralama isteği için validasyon kuralları
/// </summary>
public sealed class UpdateProductImageOrderRequestValidator : AbstractValidator<UpdateProductImageOrderRequest>
{
	public UpdateProductImageOrderRequestValidator()
	{
		RuleFor(x => x.ImageId)
			.GreaterThan(0).WithMessage("Geçerli bir resim ID'si gereklidir");

		RuleFor(x => x.NewDisplayOrder)
			.GreaterThanOrEqualTo(0).WithMessage("Görüntüleme sırası 0'dan küçük olamaz")
			.LessThanOrEqualTo(999).WithMessage("Görüntüleme sırası 999'dan küçük olmalıdır");
	}


}
