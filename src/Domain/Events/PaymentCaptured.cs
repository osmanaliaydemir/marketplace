namespace Domain.Events;

public sealed record PaymentCaptured(long PaymentId, DateTime OccurredAtUtc);
