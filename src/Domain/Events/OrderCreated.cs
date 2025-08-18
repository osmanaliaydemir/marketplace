namespace Domain.Events;

public sealed record OrderCreated(long OrderId, DateTime OccurredAtUtc);
