using FluentValidation;
using Application.DTOs.Products;

namespace Api.Validators;

/// <summary>
/// Ürün resmi güncelleme isteği için validasyon kuralları
/// </summary>
public sealed class UpdateProductImageRequestValidator : AbstractValidator<UpdateProductImageRequest>
{
	public UpdateProductImageRequestValidator()
	{
		RuleFor(x => x.ImageUrl)
			.NotEmpty().WithMessage("Resim URL'si gereklidir")
			.MaximumLength(500).WithMessage("Resim URL'si en fazla 500 karakter olabilir")
			.Must(BeValidUrl).WithMessage("Geçerli bir resim URL'si giriniz");

		RuleFor(x => x.ThumbnailUrl)
			.MaximumLength(500).WithMessage("Thumbnail URL'si en fazla 500 karakter olabilir")
			.Must(BeValidUrl).WithMessage("Geçerli bir thumbnail URL'si giriniz")
			.When(x => !string.IsNullOrWhiteSpace(x.ThumbnailUrl));

		RuleFor(x => x.AltText)
			.MaximumLength(200).WithMessage("Alt text en fazla 200 karakter olabilir")
			.When(x => !string.IsNullOrWhiteSpace(x.AltText));

		RuleFor(x => x.Title)
			.MaximumLength(100).WithMessage("Resim başlığı en fazla 100 karakter olabilir")
			.When(x => !string.IsNullOrWhiteSpace(x.Title));

		RuleFor(x => x.DisplayOrder)
			.GreaterThanOrEqualTo(0).WithMessage("Görüntüleme sırası 0 veya daha büyük olmalıdır")
			.LessThanOrEqualTo(999).WithMessage("Görüntüleme sırası 999'dan küçük olmalıdır");

		RuleFor(x => x.IsPrimary)
			.NotNull().WithMessage("Primary resim bilgisi gereklidir");

		RuleFor(x => x.IsActive)
			.NotNull().WithMessage("Aktif durum bilgisi gereklidir");
	}

	/// <summary>
	/// URL'nin geçerli olup olmadığını kontrol et
	/// </summary>
	/// <param name="url">Kontrol edilecek URL</param>
	/// <returns>URL geçerli mi?</returns>
	private static bool BeValidUrl(string? url)
	{
		if (string.IsNullOrWhiteSpace(url)) return true;
		return Uri.TryCreate(url, UriKind.Absolute, out _);
	}
}
