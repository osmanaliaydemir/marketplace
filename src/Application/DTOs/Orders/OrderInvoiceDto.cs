namespace Application.DTOs.Orders;

public sealed record OrderInvoiceDto
{
    public long Id { get; init; }
    public long OrderId { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public DateTime InvoiceDate { get; init; }
    public DateTime DueDate { get; init; }
    public decimal SubTotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal ShippingAmount { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; } = "TRY";
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? PaidAt { get; init; }
}
