using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Orders;

public record OrderUpdateRequest
{
    [StringLength(100, ErrorMessage = "Kargo adresi en fazla 100 karakter olabilir")]
    public string? ShippingAddress { get; init; }
    
    [StringLength(100, ErrorMessage = "Fatura adresi en fazla 100 karakter olabilir")]
    public string? BillingAddress { get; init; }
    
    [StringLength(50, ErrorMessage = "Telefon en fazla 50 karakter olabilir")]
    public string? Phone { get; init; }
    
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
    [StringLength(255, ErrorMessage = "Email en fazla 255 karakter olabilir")]
    public string? Email { get; init; }
    
    [StringLength(1000, ErrorMessage = "Notlar en fazla 1000 karakter olabilir")]
    public string? Notes { get; init; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Vergi tutarı 0'dan küçük olamaz")]
    public decimal? TaxAmount { get; init; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Kargo tutarı 0'dan küçük olamaz")]
    public decimal? ShippingAmount { get; init; }
    
    [Range(0, double.MaxValue, ErrorMessage = "İndirim tutarı 0'dan küçük olamaz")]
    public decimal? DiscountAmount { get; init; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Toplam tutar 0'dan küçük olamaz")]
    public decimal? TotalAmount { get; init; }
    
    [StringLength(3, ErrorMessage = "Para birimi en fazla 3 karakter olabilir")]
    public string? Currency { get; init; }
}
