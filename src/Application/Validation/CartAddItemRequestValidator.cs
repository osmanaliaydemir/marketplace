using Application.DTOs.Cart;
using FluentValidation;

namespace Application.Validation;

public sealed class CartAddItemRequestValidator : AbstractValidator<CartAddItemRequest>
{
    public CartAddItemRequestValidator()
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

        // Business Rules
        RuleFor(x => x)
            .Must(x => x.Quantity <= 100)
            .WithMessage("Tek seferde en fazla 100 adet ürün eklenebilir");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Note) || x.Note?.Length <= 500)
            .WithMessage("Not çok uzun olamaz");
    }
}
