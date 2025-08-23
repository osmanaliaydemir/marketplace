namespace Infrastructure.Persistence.Outbox;

public sealed class OutboxMessage : Domain.Models.BaseEntity
{
    public string Type { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Failed
    public int RetryCount { get; set; } = 0;
    public DateTime? ProcessedAt { get; set; }
    public string? ErrorMessage { get; set; }
}
