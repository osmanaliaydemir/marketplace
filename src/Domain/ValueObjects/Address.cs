namespace Domain.ValueObjects;

public sealed class Address
{
    public string FullName { get; set; } = string.Empty;
    public string Line1 { get; set; } = string.Empty;
    public string? Line2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Country { get; set; } = "TR";
    public string Zip { get; set; } = string.Empty;
}
