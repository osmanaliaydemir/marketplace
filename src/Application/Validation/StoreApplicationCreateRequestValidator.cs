using Application.DTOs.Stores;
using FluentValidation;

namespace Application.Validation;

public sealed class StoreApplicationCreateRequestValidator : AbstractValidator<StoreApplicationCreateRequest>
{
	public StoreApplicationCreateRequestValidator()
	{
		RuleFor(x => x.BusinessName)
			.NotEmpty().WithMessage("İşletme adı boş olamaz")
			.MinimumLength(3).WithMessage("İşletme adı en az 3 karakter olmalıdır")
			.MaximumLength(100).WithMessage("İşletme adı en fazla 100 karakter olabilir")
			.Matches(@"^[a-zA-ZğüşıöçĞÜŞIÖÇ\s]+$").WithMessage("İşletme adı sadece harf ve boşluk içerebilir");

		RuleFor(x => x.BusinessType)
			.NotEmpty().WithMessage("İşletme türü seçilmelidir")
			.MaximumLength(50).WithMessage("İşletme türü en fazla 50 karakter olabilir");

		RuleFor(x => x.BusinessDescription)
			.NotEmpty().WithMessage("İşletme açıklaması boş olamaz")
			.MinimumLength(10).WithMessage("İşletme açıklaması en az 10 karakter olmalıdır")
			.MaximumLength(500).WithMessage("İşletme açıklaması en fazla 500 karakter olabilir");

		RuleFor(x => x.PrimaryCategory)
			.NotEmpty().WithMessage("Ana kategori seçilmelidir");

		RuleFor(x => x.ProductCount)
			.NotEmpty().WithMessage("Ürün sayısı seçilmelidir");

		RuleFor(x => x.ExpectedRevenue)
			.NotEmpty().WithMessage("Beklenen gelir seçilmelidir");

		RuleFor(x => x.Experience)
			.NotEmpty().WithMessage("E-ticaret deneyimi seçilmelidir")
			.MaximumLength(100).WithMessage("E-ticaret deneyimi en fazla 100 karakter olabilir");

		RuleFor(x => x.ContactName)
			.NotEmpty().WithMessage("İletişim kişisi boş olamaz")
			.MinimumLength(2).WithMessage("İletişim kişisi en az 2 karakter olmalıdır")
			.MaximumLength(100).WithMessage("İletişim kişisi en fazla 100 karakter olabilir");

		RuleFor(x => x.PhoneNumber)
			.NotEmpty().WithMessage("Telefon numarası boş olamaz")
			.MaximumLength(20).WithMessage("Telefon numarası en fazla 20 karakter olabilir")
			.Matches(@"^[0-9\s\-\+\(\)]+$").WithMessage("Telefon numarası sadece rakam, boşluk, tire, artı ve parantez içerebilir");

		RuleFor(x => x.City)
			.NotEmpty().WithMessage("Şehir seçilmelidir");

		RuleFor(x => x.TermsAccepted)
			.Equal(true).WithMessage("Kullanım şartlarını kabul etmelisiniz");

		RuleFor(x => x.TaxNumber)
			.MaximumLength(20).When(x => !string.IsNullOrEmpty(x.TaxNumber))
			.Matches(@"^[0-9]{10,11}$").When(x => !string.IsNullOrEmpty(x.TaxNumber))
			.WithMessage("Vergi numarası 10 veya 11 haneli rakam olmalıdır");

		RuleFor(x => x.BusinessLicense)
			.MaximumLength(50).When(x => !string.IsNullOrEmpty(x.BusinessLicense))
			.WithMessage("İşletme belgesi en fazla 50 karakter olabilir");

		RuleFor(x => x.Website)
			.Must(url => string.IsNullOrEmpty(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
			.When(x => !string.IsNullOrEmpty(x.Website))
			.WithMessage("Geçerli bir web sitesi URL'si giriniz");

		RuleFor(x => x.SocialMedia)
			.MaximumLength(100).When(x => !string.IsNullOrEmpty(x.SocialMedia))
			.WithMessage("Sosyal medya bilgisi en fazla 100 karakter olabilir");

		RuleFor(x => x.SecondaryCategory)
			.MaximumLength(50).When(x => !string.IsNullOrEmpty(x.SecondaryCategory))
			.WithMessage("İkincil kategori en fazla 50 karakter olabilir");

		RuleFor(x => x.ProductDescription)
			.MaximumLength(300).When(x => !string.IsNullOrEmpty(x.ProductDescription))
			.WithMessage("Ürün açıklaması en fazla 300 karakter olabilir");

		RuleFor(x => x.Address)
			.MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Address))
			.WithMessage("Adres en fazla 200 karakter olabilir");
	}
}
