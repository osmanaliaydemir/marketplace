namespace Application.DTOs.Customers;

public sealed record CustomerAddressDto
{
    public long Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string RecipientName { get; init; } = string.Empty;
    public string AddressLine1 { get; init; } = string.Empty;
    public string? AddressLine2 { get; init; }
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string? PostalCode { get; init; }
    public string Phone { get; init; } = string.Empty;
    public bool IsDefault { get; init; }
}
