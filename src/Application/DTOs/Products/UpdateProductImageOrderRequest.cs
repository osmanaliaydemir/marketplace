namespace Application.DTOs.Products;

public sealed record UpdateProductImageOrderRequest
{
    public List<long> ImageIds { get; init; } = new(); // Sıralanmış ID listesi
}
