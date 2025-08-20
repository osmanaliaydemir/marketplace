using Application.DTOs.Orders;
using FluentValidation;

namespace Application.Validation;

public sealed class OrderCreateRequestValidator : AbstractValidator<OrderCreateRequest>
{
    public OrderCreateRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("Geçerli bir müşteri seçilmelidir");

        RuleFor(x => x.StoreId)
            .GreaterThan(0).WithMessage("Geçerli bir mağaza seçilmelidir");

        RuleFor(x => x.SubTotal)
            .GreaterThan(0).WithMessage("Alt toplam 0'dan büyük olmalıdır");

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0).WithMessage("Toplam tutar 0'dan büyük olmalıdır");

        RuleFor(x => x.ShippingAddress)
            .MaximumLength(100).WithMessage("Kargo adresi en fazla 100 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.ShippingAddress));

        RuleFor(x => x.BillingAddress)
            .MaximumLength(100).WithMessage("Fatura adresi en fazla 100 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.BillingAddress));

        RuleFor(x => x.Phone)
            .MaximumLength(50).WithMessage("Telefon en fazla 50 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Geçerli bir email adresi giriniz")
            .MaximumLength(255).WithMessage("Email en fazla 255 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notlar en fazla 1000 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.Notes));

        // Business Rules
        RuleFor(x => x)
            .Must(x => x.TotalAmount >= x.SubTotal)
            .WithMessage("Toplam tutar alt toplamdan küçük olamaz");

        RuleFor(x => x)
            .Must(x => x.TotalAmount >= x.SubTotal + x.TaxAmount + x.ShippingAmount - x.DiscountAmount)
            .WithMessage("Toplam tutar hesaplanan tutarla uyuşmuyor");
    }
}

public sealed class AddressValidator : AbstractValidator<AddressDto>
{
    public AddressValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Ad soyad boş olamaz")
            .MaximumLength(100).WithMessage("Ad soyad en fazla 100 karakter olabilir")
            .MinimumLength(2).WithMessage("Ad soyad en az 2 karakter olmalıdır");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefon boş olamaz")
            .MaximumLength(20).WithMessage("Telefon en fazla 20 karakter olabilir")
            .Matches(@"^[\+]?[0-9\s\-\(\)]+$").WithMessage("Geçerli bir telefon numarası giriniz");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Adres boş olamaz")
            .MaximumLength(500).WithMessage("Adres en fazla 500 karakter olabilir")
            .MinimumLength(10).WithMessage("Adres en az 10 karakter olmalıdır");

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("Şehir en fazla 100 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.City));

        RuleFor(x => x.District)
            .MaximumLength(100).WithMessage("İlçe en fazla 100 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.District));

        RuleFor(x => x.PostalCode)
            .MaximumLength(10).WithMessage("Posta kodu en fazla 10 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.PostalCode));

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Ülke boş olamaz")
            .MaximumLength(100).WithMessage("Ülke en fazla 100 karakter olabilir");
    }
}

public sealed class ContactInfoValidator : AbstractValidator<ContactInfoDto>
{
    public ContactInfoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta boş olamaz")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz")
            .MaximumLength(255).WithMessage("E-posta en fazla 255 karakter olabilir");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefon boş olamaz")
            .MaximumLength(20).WithMessage("Telefon en fazla 20 karakter olabilir")
            .Matches(@"^[\+]?[0-9\s\-\(\)]+$").WithMessage("Geçerli bir telefon numarası giriniz");
    }
}

public sealed class OrderItemValidator : AbstractValidator<OrderItemRequest>
{
    public OrderItemValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Geçerli bir ürün seçilmelidir");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Miktar 0'dan büyük olmalıdır")
            .LessThanOrEqualTo(100).WithMessage("Miktar 100'den büyük olamaz");

        RuleFor(x => x.ProductVariantId)
            .GreaterThan(0).WithMessage("Geçerli bir ürün varyantı seçilmelidir")
            .When(x => x.ProductVariantId.HasValue);

        RuleFor(x => x.Note)
            .MaximumLength(500).WithMessage("Not en fazla 500 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.Note));
    }
}
