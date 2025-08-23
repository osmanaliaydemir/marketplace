namespace Domain.Entities;

public sealed class WebhookDelivery : Domain.Models.BaseEntity
{
    public string WebhookUrl { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, Sent, Failed
    public int RetryCount { get; set; } = 0;
    public DateTime? SentAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int ResponseCode { get; set; }
    public string? ResponseBody { get; set; }
    
    // Navigation properties
    public OutboxMessage OutboxMessage { get; set; } = null!;
}


