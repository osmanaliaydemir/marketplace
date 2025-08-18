namespace Domain.Entities;

public sealed class OutboxMessage : Domain.Models.AuditableEntity
{
    public Guid Id { get; init; }
    public string Type { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime OccurredOn { get; init; }
    public DateTime? ProcessedOn { get; set; }
    public int Retries { get; set; }
    public string? Error { get; set; }
}


