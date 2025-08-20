namespace Application.DTOs.Payments;

public sealed record PaymentMethodDto
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public decimal ProcessingFee { get; init; }
    public string ProcessingFeeType { get; init; } = string.Empty;
}
