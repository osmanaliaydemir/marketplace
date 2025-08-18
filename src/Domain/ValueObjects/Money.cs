namespace Domain.ValueObjects;

public readonly record struct Money(decimal Amount, string Currency);
