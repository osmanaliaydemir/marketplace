namespace Application.DTOs.Payments;

public sealed record PaymentStatsDto
{
    public DateTime PeriodFrom { get; init; }
    public DateTime PeriodTo { get; init; }
    public int TotalPayments { get; init; }
    public int SuccessfulPayments { get; init; }
    public int FailedPayments { get; init; }
    public int PendingPayments { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal AverageAmount { get; init; }
    public decimal SuccessRate { get; init; }
}

public sealed record PaymentMethodStatsDto
{
    public string PaymentMethod { get; init; } = string.Empty;
    public int Count { get; init; }
    public decimal Amount { get; init; }
    public decimal SuccessRate { get; init; }
}

public sealed record PaymentTrendDto
{
    public DateTime Date { get; init; }
    public int Count { get; init; }
    public decimal Amount { get; init; }
    public decimal SuccessRate { get; init; }
}
