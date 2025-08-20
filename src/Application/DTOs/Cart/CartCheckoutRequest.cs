using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Cart;

public sealed record CartCheckoutRequest
{
    [Required(ErrorMessage = "Kargo adresi zorunludur")]
    public AddressDto ShippingAddress { get; init; } = new();
    
    [Required(ErrorMessage = "Fatura adresi zorunludur")]
    public AddressDto BillingAddress { get; init; } = new();
    
    [Required(ErrorMessage = "İletişim bilgisi zorunludur")]
    public ContactInfoDto ContactInfo { get; init; } = new();
    
    [StringLength(500, ErrorMessage = "Not en fazla 500 karakter olabilir")]
    public string? Note { get; init; }
    
    [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir")]
    public string? Notes { get; init; }
    
    [StringLength(20, ErrorMessage = "Telefon en fazla 20 karakter olabilir")]
    public string? Phone { get; init; }
    
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
    public string? Email { get; init; }
    
    public string? CouponCode { get; init; }
    public string? PaymentMethod { get; init; }
    public string? ShippingMethod { get; init; }
    public bool UseBillingAddressForShipping { get; init; } = false;
    public bool AcceptTerms { get; init; }
    public bool SubscribeToNewsletter { get; init; } = false;
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
