namespace Domain.Entities;

public sealed class Seller : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long UserId { get; set; }
    public string? PaytrSubmerchantId { get; set; }
    public byte KycStatus { get; set; }
    public decimal CommissionRate { get; set; }
    public string? Iban { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; set; }
}
