using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dashboard.Services;

namespace Dashboard.Pages;

public class IndexModel : PageModel
{
    private readonly ApiClient _api;

    public IndexModel(ApiClient api)
    {
        _api = api;
    }

    public string UserName { get; set; } = "Admin User";
    public string UserRole { get; set; } = "Admin";
    public decimal TotalSales { get; set; } = 125000;
    public int TotalOrders { get; set; } = 1250;
    public int PendingApplications { get; set; } = 5;
    public int ActiveStores { get; set; } = 45;
    public List<ActivityItem> RecentActivities { get; set; } = new();

    public async Task OnGetAsync()
    {
        // Session'dan kullanıcı bilgilerini al
        var userName = HttpContext.Session.GetString("UserName");
        var userRole = HttpContext.Session.GetString("UserRole");
        
        if (!string.IsNullOrEmpty(userName))
        {
            UserName = userName;
        }
        
        if (!string.IsNullOrEmpty(userRole))
        {
            UserRole = userRole;
        }

        // API'den dashboard verilerini çek
        try
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (!string.IsNullOrEmpty(token))
            {
                _api.SetBearer(token);
                
                // Dashboard istatistiklerini al
                var stats = await _api.GetAsync<DashboardStats>("/api/dashboard/stats");
                if (stats != null)
                {
                    TotalSales = stats.TotalSales;
                    TotalOrders = stats.TotalOrders;
                    PendingApplications = stats.PendingApplications;
                    ActiveStores = stats.ActiveStores;
                }

                // Son aktiviteleri al
                var activities = await _api.GetAsync<IEnumerable<ActivityItem>>("/api/dashboard/recent-activities");
                if (activities != null)
                {
                    RecentActivities = activities.ToList();
                }
            }
        }
        catch
        {
            // API hatası durumunda varsayılan veriler kullanılıyor
        }

        // Eğer aktivite yoksa örnek veriler göster
        if (!RecentActivities.Any())
        {
            RecentActivities = GetSampleActivities();
        }
    }

    private List<ActivityItem> GetSampleActivities()
    {
        return new List<ActivityItem>
        {
            new ActivityItem
            {
                Title = "Yeni Mağaza Başvurusu",
                Description = "TechStore mağazası başvuru yaptı",
                Icon = "bi bi-shop",
                TimeAgo = "2 saat önce"
            },
            new ActivityItem
            {
                Title = "Sipariş Tamamlandı",
                Description = "#ORD-001234 siparişi başarıyla tamamlandı",
                Icon = "bi bi-check-circle",
                TimeAgo = "4 saat önce"
            },
            new ActivityItem
            {
                Title = "Yeni Ürün Eklendi",
                Description = "Electronics kategorisine 15 yeni ürün eklendi",
                Icon = "bi bi-box",
                TimeAgo = "6 saat önce"
            },
            new ActivityItem
            {
                Title = "Ödeme Alındı",
                Description = "₺2,450 tutarında ödeme başarıyla alındı",
                Icon = "bi bi-credit-card",
                TimeAgo = "8 saat önce"
            }
        };
    }
}

public sealed class DashboardStats
{
    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public int PendingApplications { get; set; }
    public int ActiveStores { get; set; }
}

public sealed class ActivityItem
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string TimeAgo { get; set; } = string.Empty;
}
