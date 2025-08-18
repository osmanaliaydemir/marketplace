namespace Domain.Entities;

public sealed class WebhookDelivery : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public string? Signature { get; set; }
    public string? Status { get; set; }
    public int Attempts { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? DeliveredAt { get; set; }
    public string? ExternalId { get; set; }
    public string? MerchantOid { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? Error { get; set; }
}


