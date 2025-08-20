using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Orders;

public record OrderCreateRequest
{
    [Required(ErrorMessage = "Müşteri ID zorunludur")]
    public long CustomerId { get; init; }
    
    [Required(ErrorMessage = "Mağaza ID zorunludur")]
    public long StoreId { get; init; }
    
    [Required(ErrorMessage = "Alt toplam zorunludur")]
    [Range(0, double.MaxValue, ErrorMessage = "Alt toplam 0'dan küçük olamaz")]
    public decimal SubTotal { get; init; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Vergi tutarı 0'dan küçük olamaz")]
    public decimal TaxAmount { get; init; } = 0;
    
    [Range(0, double.MaxValue, ErrorMessage = "Kargo tutarı 0'dan küçük olamaz")]
    public decimal ShippingAmount { get; init; } = 0;
    
    [Range(0, double.MaxValue, ErrorMessage = "İndirim tutarı 0'dan küçük olamaz")]
    public decimal DiscountAmount { get; init; } = 0;
    
    [Required(ErrorMessage = "Toplam tutar zorunludur")]
    [Range(0, double.MaxValue, ErrorMessage = "Toplam tutar 0'dan küçük olamaz")]
    public decimal TotalAmount { get; init; }
    
    [StringLength(3, ErrorMessage = "Para birimi en fazla 3 karakter olabilir")]
    public string Currency { get; init; } = "TRY";
    
    [StringLength(1000, ErrorMessage = "Notlar en fazla 1000 karakter olabilir")]
    public string? Notes { get; init; }
    
    [StringLength(100, ErrorMessage = "Kargo adresi en fazla 100 karakter olabilir")]
    public string? ShippingAddress { get; init; }
    
    [StringLength(100, ErrorMessage = "Fatura adresi en fazla 100 karakter olabilir")]
    public string? BillingAddress { get; init; }
    
    [StringLength(50, ErrorMessage = "Telefon en fazla 50 karakter olabilir")]
    public string? Phone { get; init; }
    
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
    [StringLength(255, ErrorMessage = "Email en fazla 255 karakter olabilir")]
    public string? Email { get; init; }
}

public sealed record AddressDto
{
    [Required(ErrorMessage = "Ad soyad zorunludur")]
    [StringLength(100, ErrorMessage = "Ad soyad en fazla 100 karakter olabilir")]
    public string FullName { get; init; } = string.Empty;
    
    [Required(ErrorMessage = "Telefon zorunludur")]
    [StringLength(20, ErrorMessage = "Telefon en fazla 20 karakter olabilir")]
    public string Phone { get; init; } = string.Empty;
    
    [Required(ErrorMessage = "Adres zorunludur")]
    [StringLength(500, ErrorMessage = "Adres en fazla 500 karakter olabilir")]
    public string Address { get; init; } = string.Empty;
    
    [StringLength(100, ErrorMessage = "Şehir en fazla 100 karakter olabilir")]
    public string? City { get; init; }
    
    [StringLength(100, ErrorMessage = "İlçe en fazla 100 karakter olabilir")]
    public string? District { get; init; }
    
    [StringLength(10, ErrorMessage = "Posta kodu en fazla 10 karakter olabilir")]
    public string? PostalCode { get; init; }
    
    [StringLength(100, ErrorMessage = "Ülke en fazla 100 karakter olabilir")]
    public string Country { get; init; } = "Türkiye";
}

public sealed record ContactInfoDto
{
    [Required(ErrorMessage = "E-posta zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
    public string Email { get; init; } = string.Empty;
    
    [Required(ErrorMessage = "Telefon zorunludur")]
    [StringLength(20, ErrorMessage = "Telefon en fazla 20 karakter olabilir")]
    public string Phone { get; init; } = string.Empty;
}

public sealed record OrderItemRequest
{
    [Required(ErrorMessage = "Ürün ID zorunludur")]
    public long ProductId { get; init; }
    
    [Required(ErrorMessage = "Miktar zorunludur")]
    [Range(1, int.MaxValue, ErrorMessage = "Miktar 1'den küçük olamaz")]
    public int Quantity { get; init; }
    
    public long? ProductVariantId { get; init; }
    
    [StringLength(500, ErrorMessage = "Not en fazla 500 karakter olabilir")]
    public string? Note { get; init; }
}
