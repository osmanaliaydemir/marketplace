using Application.DTOs.Stores;
using FluentValidation;

namespace Application.Validation;

public sealed class StoreApplicationCreateRequestValidator : AbstractValidator<StoreApplicationCreateRequest>
{
	public StoreApplicationCreateRequestValidator()
	{
		RuleFor(x => x.StoreName)
			.NotEmpty().WithMessage("Mağaza adı boş olamaz")
			.MinimumLength(3).WithMessage("Mağaza adı en az 3 karakter olmalıdır")
			.MaximumLength(255).WithMessage("Mağaza adı en fazla 255 karakter olabilir")
			.Matches(@"^[a-zA-ZğüşıöçĞÜŞIÖÇ\s]+$").WithMessage("Mağaza adı sadece harf ve boşluk içerebilir");

		RuleFor(x => x.Slug)
			.NotEmpty().WithMessage("Slug boş olamaz")
			.MinimumLength(3).WithMessage("Slug en az 3 karakter olmalıdır")
			.MaximumLength(100).WithMessage("Slug en fazla 100 karakter olabilir")
			.Matches(@"^[a-z0-9-]+$").WithMessage("Slug sadece küçük harf, rakam ve tire içerebilir")
			.Must(slug => !slug.StartsWith("-") && !slug.EndsWith("-")).WithMessage("Slug tire ile başlayamaz veya bitemez");

		RuleFor(x => x.ContactEmail)
			.NotEmpty().WithMessage("İletişim e-postası boş olamaz")
			.EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz")
			.MaximumLength(255).WithMessage("E-posta adresi en fazla 255 karakter olabilir");

		RuleFor(x => x.ContactPhone)
			.MaximumLength(20).WithMessage("Telefon numarası en fazla 20 karakter olabilir")
			.Matches(@"^[0-9\s\-\+\(\)]+$").When(x => !string.IsNullOrEmpty(x.ContactPhone))
			.WithMessage("Telefon numarası sadece rakam, boşluk, tire, artı ve parantez içerebilir");

		RuleFor(x => x.Description)
			.MaximumLength(2000).When(x => !string.IsNullOrEmpty(x.Description))
			.WithMessage("Açıklama en fazla 2000 karakter olabilir");

		RuleFor(x => x.BusinessAddress)
			.MaximumLength(500).When(x => !string.IsNullOrEmpty(x.BusinessAddress))
			.WithMessage("İş adresi en fazla 500 karakter olabilir");

		RuleFor(x => x.TaxNumber)
			.MaximumLength(20).When(x => !string.IsNullOrEmpty(x.TaxNumber))
			.Matches(@"^[0-9]{10,11}$").When(x => !string.IsNullOrEmpty(x.TaxNumber))
			.WithMessage("Vergi numarası 10 veya 11 haneli rakam olmalıdır");
	}
}
