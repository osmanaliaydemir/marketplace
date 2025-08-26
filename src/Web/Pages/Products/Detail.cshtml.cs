using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.DTOs.Products;

namespace Web.Pages.Products;

public class ProductDetailModel : PageModel
{
    public ProductDetailDto? Product { get; set; }
    
    public async Task OnGetAsync(int id)
    {
        // TODO: Replace with actual API call to get product details
        // For now, create a mock product for demonstration
        Product = CreateMockProduct(id);
    }
    
    private ProductDetailDto CreateMockProduct(int id)
    {
        return new ProductDetailDto
        {
            Id = id,
            Name = $"Örnek Ürün {id}",
            Description = "Bu ürün hakkında detaylı açıklama burada yer alacak. Ürünün özellikleri, kullanım alanları ve diğer önemli bilgiler bu bölümde bulunacak.",
            ShortDescription = "Kısa ürün açıklaması burada yer alacak.",
            Price = 299.99m,
            CompareAtPrice = 399.99m,
            Currency = "TRY",
            StockQty = 50,
            Weight = 1.5m,
            MinOrderQty = 1,
            MaxOrderQty = 10,
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = DateTime.Now.AddDays(-30),
            UpdatedAt = DateTime.Now,
            Slug = $"ornek-urun-{id}",
            MetaTitle = $"Örnek Ürün {id} - Meta Başlık",
            MetaDescription = $"Örnek Ürün {id} için meta açıklama",
            MetaKeywords = "örnek, ürün, meta, anahtar kelimeler",
            Rating = 4.5m,
            ReviewCount = 128,
            Brand = "TechBrand",
            Model = "TB-2024",
            Sku = $"SKU-{id}",
            Barcode = $"BAR-{id}",
            Category = new Application.DTOs.Categories.CategoryDto
            {
                Id = 1,
                Name = "Elektronik",
                Slug = "elektronik",
                Description = "Elektronik ürünler kategorisi",
                IsActive = true
            },
            Store = new Application.DTOs.Stores.StoreDto
            {
                Id = 1,
                Name = "TechStore",
                Slug = "techstore",
                Description = "Teknoloji ürünleri mağazası",
                IsActive = true
            },
            Seller = new Application.DTOs.Sellers.SellerDto
            {
                Id = 1,
                UserId = 1,
                CommissionRate = 10.0m,
                IsActive = true,
                User = new Application.DTOs.Users.AppUserDto
                {
                    Id = 1,
                    UserName = "techseller",
                    Email = "info@techseller.com",
                    PhoneNumber = "+90 212 123 45 67",
                    FirstName = "Tech",
                    LastName = "Seller"
                }
            },
            Images = new List<ProductImageDto>
            {
                new ProductImageDto
                {
                    Id = 1,
                    ImageUrl = "https://via.placeholder.com/600x600/0d6efd/ffffff?text=Product+Image+1",
                    AltText = "Ürün Resmi 1",
                    Title = "Ürün Resmi 1",
                    DisplayOrder = 1,
                    IsPrimary = true,
                    IsActive = true
                },
                new ProductImageDto
                {
                    Id = 2,
                    ImageUrl = "https://via.placeholder.com/600x600/198754/ffffff?text=Product+Image+2",
                    AltText = "Ürün Resmi 2",
                    Title = "Ürün Resmi 2",
                    DisplayOrder = 2,
                    IsPrimary = false,
                    IsActive = true
                },
                new ProductImageDto
                {
                    Id = 3,
                    ImageUrl = "https://via.placeholder.com/600x600/dc3545/ffffff?text=Product+Image+3",
                    AltText = "Ürün Resmi 3",
                    Title = "Ürün Resmi 3",
                    DisplayOrder = 3,
                    IsPrimary = false,
                    IsActive = true
                },
                new ProductImageDto
                {
                    Id = 4,
                    ImageUrl = "https://via.placeholder.com/600x600/ffc107/000000?text=Product+Image+4",
                    AltText = "Ürün Resmi 4",
                    Title = "Ürün Resmi 4",
                    DisplayOrder = 4,
                    IsPrimary = false,
                    IsActive = true
                }
            },
            Variants = new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    Id = 1,
                    VariantName = "Kırmızı",
                    Sku = $"SKU-{id}-RED",
                    Barcode = $"BAR-{id}-RED",
                    Price = 299.99m,
                    CompareAtPrice = 399.99m,
                    StockQty = 25,
                    MinOrderQty = 1,
                    MaxOrderQty = 10,
                    Weight = 1.5m,
                    DisplayOrder = 1,
                    IsDefault = true,
                    IsActive = true
                },
                new ProductVariantDto
                {
                    Id = 2,
                    VariantName = "Mavi",
                    Sku = $"SKU-{id}-BLUE",
                    Barcode = $"BAR-{id}-BLUE",
                    Price = 299.99m,
                    CompareAtPrice = 399.99m,
                    StockQty = 15,
                    MinOrderQty = 1,
                    MaxOrderQty = 10,
                    Weight = 1.5m,
                    DisplayOrder = 2,
                    IsDefault = false,
                    IsActive = true
                },
                new ProductVariantDto
                {
                    Id = 3,
                    VariantName = "Yeşil",
                    Sku = $"SKU-{id}-GREEN",
                    Barcode = $"BAR-{id}-GREEN",
                    Price = 299.99m,
                    CompareAtPrice = 399.99m,
                    StockQty = 10,
                    MinOrderQty = 1,
                    MaxOrderQty = 10,
                    Weight = 1.5m,
                    DisplayOrder = 3,
                    IsDefault = false,
                    IsActive = true
                }
            },
            Reviews = new List<Application.DTOs.Products.ProductReviewDto>
            {
                new Application.DTOs.Products.ProductReviewDto
                {
                    Id = 1,
                    CustomerName = "Ahmet Yılmaz",
                    Rating = 5,
                    Comment = "Harika bir ürün! Çok memnun kaldım. Kesinlikle tavsiye ederim.",
                    CreatedAt = DateTime.Now.AddDays(-5)
                },
                new Application.DTOs.Products.ProductReviewDto
                {
                    Id = 2,
                    CustomerName = "Ayşe Demir",
                    Rating = 4,
                    Comment = "Kaliteli ürün, hızlı kargo. Sadece biraz daha büyük olabilirdi.",
                    CreatedAt = DateTime.Now.AddDays(-10)
                },
                new Application.DTOs.Products.ProductReviewDto
                {
                    Id = 3,
                    CustomerName = "Mehmet Kaya",
                    Rating = 5,
                    Comment = "Mükemmel! Tam beklediğim gibi. Teşekkürler.",
                    CreatedAt = DateTime.Now.AddDays(-15)
                }
            }
        };
    }
}
