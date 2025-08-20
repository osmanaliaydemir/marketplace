using Application.DTOs.Payments;
using FluentValidation;

namespace Application.Validation;

public sealed class PaymentInitiationRequestValidator : AbstractValidator<PaymentInitiationRequest>
{
    public PaymentInitiationRequestValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0).WithMessage("Sipariş ID geçerli olmalıdır");

        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("Müşteri ID geçerli olmalıdır");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Tutar 0'dan büyük olmalıdır");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("Ödeme yöntemi boş olamaz");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Para birimi boş olamaz")
            .When(x => !string.IsNullOrEmpty(x.Currency));
    }
}