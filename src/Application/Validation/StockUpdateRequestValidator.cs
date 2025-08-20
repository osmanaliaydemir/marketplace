using Application.DTOs.Inventory;
using FluentValidation;

namespace Application.Validation;

public sealed class StockUpdateRequestValidator : AbstractValidator<StockUpdateRequest>
{
    public StockUpdateRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Geçerli bir ürün seçilmelidir");

        RuleFor(x => x.Quantity)
            .NotEqual(0).WithMessage("Miktar 0 olamaz")
            .When(x => x.OperationType == StockOperationType.Adjustment);

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Miktar 0'dan büyük olmalıdır")
            .When(x => x.OperationType != StockOperationType.Adjustment);

        RuleFor(x => x.OperationType)
            .IsInEnum().WithMessage("Geçerli bir stok işlem türü seçilmelidir")
            .NotEqual(StockOperationType.Unknown).WithMessage("Stok işlem türü bilinmiyor");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("İşlem nedeni boş olamaz")
            .MaximumLength(500).WithMessage("İşlem nedeni en fazla 500 karakter olabilir")
            .When(x => x.OperationType == StockOperationType.Adjustment || 
                       x.OperationType == StockOperationType.Damage || 
                       x.OperationType == StockOperationType.Expiry);

        RuleFor(x => x.Reference)
            .MaximumLength(100).WithMessage("Referans en fazla 100 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.Reference));

        RuleFor(x => x.OrderId)
            .GreaterThan(0).WithMessage("Geçerli bir sipariş seçilmelidir")
            .When(x => x.OperationType == StockOperationType.Sale || 
                       x.OperationType == StockOperationType.Return ||
                       x.OperationType == StockOperationType.Reservation ||
                       x.OperationType == StockOperationType.Release);

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("Geçerli bir kullanıcı seçilmelidir")
            .When(x => x.OperationType == StockOperationType.Adjustment ||
                       x.OperationType == StockOperationType.Damage ||
                       x.OperationType == StockOperationType.Expiry);

        // Business Rules
        RuleFor(x => x)
            .Must(x => x.Quantity <= 100000)
            .WithMessage("Tek seferde en fazla 100,000 adet stok işlemi yapılabilir");

        RuleFor(x => x)
            .Must(x => x.Quantity >= -100000)
            .WithMessage("Tek seferde en fazla 100,000 adet stok çıkarılabilir")
            .When(x => x.OperationType == StockOperationType.Subtraction || 
                       x.OperationType == StockOperationType.Adjustment);

        RuleFor(x => x)
            .Must(x => x.OperationType != StockOperationType.Reservation || x.Quantity > 0)
            .WithMessage("Rezervasyon miktarı pozitif olmalıdır");

        RuleFor(x => x)
            .Must(x => x.OperationType != StockOperationType.Release || x.Quantity > 0)
            .WithMessage("Serbest bırakma miktarı pozitif olmalıdır");

        // Complex Business Rules
        RuleFor(x => x)
            .Must(ValidateOperationTypeAndQuantity)
            .WithMessage("İşlem türü ve miktar uyumsuz");

        RuleFor(x => x)
            .Must(ValidateRequiredFields)
            .WithMessage("Bu işlem türü için gerekli alanlar eksik");
    }

    private static bool ValidateOperationTypeAndQuantity(StockUpdateRequest request)
    {
        return request.OperationType switch
        {
            StockOperationType.Addition => request.Quantity > 0,
            StockOperationType.Subtraction => request.Quantity > 0,
            StockOperationType.Adjustment => true, // Can be positive or negative
            StockOperationType.Reservation => request.Quantity > 0,
            StockOperationType.Release => request.Quantity > 0,
            StockOperationType.Sale => request.Quantity > 0,
            StockOperationType.Return => request.Quantity > 0,
            StockOperationType.Damage => request.Quantity > 0,
            StockOperationType.Expiry => request.Quantity > 0,
            _ => false
        };
    }

    private static bool ValidateRequiredFields(StockUpdateRequest request)
    {
        return request.OperationType switch
        {
            StockOperationType.Adjustment => !string.IsNullOrEmpty(request.Reason),
            StockOperationType.Damage => !string.IsNullOrEmpty(request.Reason),
            StockOperationType.Expiry => !string.IsNullOrEmpty(request.Reason),
            StockOperationType.Sale => request.OrderId.HasValue,
            StockOperationType.Return => request.OrderId.HasValue,
            StockOperationType.Reservation => request.OrderId.HasValue,
            StockOperationType.Release => request.OrderId.HasValue,
            _ => true
        };
    }
}
