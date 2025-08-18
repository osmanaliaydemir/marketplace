namespace Infrastructure.Persistence.Outbox;

public sealed class OutboxMessage
{
    public long Id { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Payload { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; set; }
}
