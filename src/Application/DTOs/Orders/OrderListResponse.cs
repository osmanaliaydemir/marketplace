namespace Application.DTOs.Orders;

public record OrderListResponse
{
    public IEnumerable<OrderDto> Orders { get; init; } = Enumerable.Empty<OrderDto>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }
}


