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
		RuleFor(x => x.ImageIds)
			.NotEmpty().WithMessage("Resim ID listesi boş olamaz")
			.Must(HaveUniqueIds).WithMessage("Resim ID'leri benzersiz olmalıdır")
			.Must(HaveValidIds).WithMessage("Geçersiz resim ID'leri bulunmaktadır");

		RuleFor(x => x.ImageIds.Count)
			.GreaterThan(0).WithMessage("En az bir resim ID'si gereklidir")
			.LessThanOrEqualTo(50).WithMessage("Maksimum 50 resim sıralanabilir");
	}

	/// <summary>
	/// ID'lerin benzersiz olup olmadığını kontrol et
	/// </summary>
	/// <param name="imageIds">Kontrol edilecek ID listesi</param>
	/// <returns>ID'ler benzersiz mi?</returns>
	private static bool HaveUniqueIds(List<long> imageIds)
	{
		if (imageIds == null) return false;
		return imageIds.Count == imageIds.Distinct().Count();
	}

	/// <summary>
	/// ID'lerin geçerli olup olmadığını kontrol et
	/// </summary>
	/// <param name="imageIds">Kontrol edilecek ID listesi</param>
	/// <returns>ID'ler geçerli mi?</returns>
	private static bool HaveValidIds(List<long> imageIds)
	{
		if (imageIds == null) return false;
		return imageIds.All(id => id > 0);
	}
}
