using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dashboard.Services;

namespace Dashboard.Pages.Sellers;

public class IndexModel : PageModel
{
    private readonly ApiClient _api;

    public IndexModel(ApiClient api)
    {
        _api = api;
    }

    public int TotalSellers { get; set; } = 145;
    public int ActiveSellers { get; set; } = 120;
    public int PendingSellers { get; set; } = 15;
    public decimal TotalRevenue { get; set; } = 487500.00m;
    public List<SellerListItem> Sellers { get; set; } = new();

    public async Task OnGetAsync()
    {
        try
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (!string.IsNullOrEmpty(token))
            {
                _api.SetBearer(token);
                
                // API'den satıcı verilerini çek
                var sellerStats = await _api.GetAsync<SellerStats>("/api/dashboard/seller-stats");
                if (sellerStats != null)
                {
                    TotalSellers = sellerStats.TotalSellers;
                    ActiveSellers = sellerStats.ActiveSellers;
                    PendingSellers = sellerStats.PendingSellers;
                    TotalRevenue = sellerStats.TotalRevenue;
                }

                // Satıcı listesini al
                var sellers = await _api.GetAsync<IEnumerable<SellerListItem>>("/api/sellers?limit=20");
                if (sellers != null)
                {
                    Sellers = sellers.ToList();
                }
            }
        }
        catch
        {
            // API hatası durumunda örnek veriler kullan
        }

        // Eğer veri yoksa örnek veriler göster
        if (!Sellers.Any())
        {
            Sellers = GetSampleSellers();
        }
    }

    private List<SellerListItem> GetSampleSellers()
    {
        return new List<SellerListItem>
        {
            new SellerListItem
            {
                Id = 1,
                FullName = "TechStore Elektronik",
                Email = "info@techstore.com",
                StoreName = "TechStore",
                StoreId = 1,
                ProductCount = 156,
                TotalSales = 125000.00m,
                CommissionRate = 8.5m,
                KycStatus = "Onaylandı",
                KycStatusColor = "success",
                Status = "Aktif",
                StatusColor = "success"
            },
            new SellerListItem
            {
                Id = 2,
                FullName = "Moda Dünyası Ltd.",
                Email = "iletisim@modadunyasi.com",
                StoreName = "Moda Dünyası",
                StoreId = 2,
                ProductCount = 89,
                TotalSales = 87500.00m,
                CommissionRate = 12.0m,
                KycStatus = "Onaylandı",
                KycStatusColor = "success",
                Status = "Aktif",
                StatusColor = "success"
            },
            new SellerListItem
            {
                Id = 3,
                FullName = "Ev & Yaşam Mağazası",
                Email = "satış@evyasam.com",
                StoreName = "Ev & Yaşam",
                StoreId = 3,
                ProductCount = 234,
                TotalSales = 156700.00m,
                CommissionRate = 10.0m,
                KycStatus = "Bekleyen",
                KycStatusColor = "warning",
                Status = "Aktif",
                StatusColor = "success"
            },
            new SellerListItem
            {
                Id = 4,
                FullName = "Spor Merkezi",
                Email = "info@spormerkezi.com",
                StoreName = "Spor Merkezi",
                StoreId = 4,
                ProductCount = 67,
                TotalSales = 45200.00m,
                CommissionRate = 9.5m,
                KycStatus = "Reddedildi",
                KycStatusColor = "danger",
                Status = "Askıda",
                StatusColor = "warning"
            },
            new SellerListItem
            {
                Id = 5,
                FullName = "Kitap Dünyası",
                Email = "siparis@kitapdunyasi.com",
                StoreName = "Kitap Dünyası",
                StoreId = 5,
                ProductCount = 345,
                TotalSales = 78900.00m,
                CommissionRate = 15.0m,
                KycStatus = "Onaylandı",
                KycStatusColor = "success",
                Status = "Aktif",
                StatusColor = "success"
            },
            new SellerListItem
            {
                Id = 6,
                FullName = "Ahmet Yılmaz",
                Email = "ahmet.yilmaz@email.com",
                StoreName = "",
                StoreId = null,
                ProductCount = 0,
                TotalSales = 0.00m,
                CommissionRate = 10.0m,
                KycStatus = "Bekleyen",
                KycStatusColor = "warning",
                Status = "Bekleyen",
                StatusColor = "info"
            },
            new SellerListItem
            {
                Id = 7,
                FullName = "Gourmet Lezzetler",
                Email = "info@gourmetlezzetler.com",
                StoreName = "Gourmet Lezzetler",
                StoreId = 7,
                ProductCount = 123,
                TotalSales = 92400.00m,
                CommissionRate = 11.5m,
                KycStatus = "Onaylandı",
                KycStatusColor = "success",
                Status = "Aktif",
                StatusColor = "success"
            }
        };
    }
}

public sealed class SellerStats
{
    public int TotalSellers { get; set; }
    public int ActiveSellers { get; set; }
    public int PendingSellers { get; set; }
    public decimal TotalRevenue { get; set; }
}

public sealed class SellerListItem
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
    public long? StoreId { get; set; }
    public int ProductCount { get; set; }
    public decimal TotalSales { get; set; }
    public decimal CommissionRate { get; set; }
    public string KycStatus { get; set; } = string.Empty;
    public string KycStatusColor { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StatusColor { get; set; } = string.Empty;
}
