namespace Application.DTOs.Customers;

public sealed record UpdateCustomerProfileRequest
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public DateTime? BirthDate { get; init; }
    public string? Gender { get; init; }
}
