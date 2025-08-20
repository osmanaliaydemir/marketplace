namespace Domain.Entities;

public sealed class Payment : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long OrderId { get; set; }
    public long CustomerId { get; set; }
    public string Provider { get; set; } = "paytr";
    public string? ProviderPaymentId { get; set; }
    public string? ProviderTx { get; set; }
    public string Status { get; set; } = "pending";
    public decimal Amount { get; set; }
    public decimal Gross { get; set; }
    public decimal FeePlatform { get; set; }
    public decimal FeePsp { get; set; }
    public decimal NetToSellers { get; set; }
    public string PaymentMethod { get; set; } = "credit_card";
    public string? TransactionId { get; set; }
    public string? RedirectUrl { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime? CapturedAt { get; set; }
    public string MerchantOid { get; set; } = string.Empty;
    public string? Currency { get; set; }
    public bool IsTest { get; set; }
}
