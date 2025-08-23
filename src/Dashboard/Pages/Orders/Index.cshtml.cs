using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dashboard.Services;

namespace Dashboard.Pages.Orders;

public class IndexModel : PageModel
{
    private readonly ApiClient _api;

    public IndexModel(ApiClient api)
    {
        _api = api;
    }

    public int TotalOrders { get; set; } = 1250;
    public int PendingOrders { get; set; } = 45;
    public int CompletedOrders { get; set; } = 1180;
    public int CancelledOrders { get; set; } = 25;
    public List<OrderListItem> Orders { get; set; } = new();

    public async Task OnGetAsync()
    {
        try
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (!string.IsNullOrEmpty(token))
            {
                _api.SetBearer(token);
                
                // API'den sipariş verilerini çek
                var orderStats = await _api.GetAsync<OrderStats>("/api/dashboard/order-stats");
                if (orderStats != null)
                {
                    TotalOrders = orderStats.TotalOrders;
                    PendingOrders = orderStats.PendingOrders;
                    CompletedOrders = orderStats.CompletedOrders;
                    CancelledOrders = orderStats.CancelledOrders;
                }

                // Son siparişleri al
                var recentOrders = await _api.GetAsync<IEnumerable<OrderListItem>>("/api/orders?limit=20");
                if (recentOrders != null)
                {
                    Orders = recentOrders.ToList();
                }
            }
        }
        catch
        {
            // API hatası durumunda örnek veriler kullan
        }

        // Eğer veri yoksa örnek veriler göster
        if (!Orders.Any())
        {
            Orders = GetSampleOrders();
        }
    }

    private List<OrderListItem> GetSampleOrders()
    {
        return new List<OrderListItem>
        {
            new OrderListItem
            {
                Id = 1,
                OrderNumber = "ORD-001234",
                CustomerName = "Ahmet Yılmaz",
                CustomerEmail = "ahmet.yilmaz@email.com",
                ItemCount = 3,
                Total = 2450.00m,
                Status = "Bekleyen",
                StatusColor = "warning",
                CreatedAt = DateTime.Now.AddHours(-2)
            },
            new OrderListItem
            {
                Id = 2,
                OrderNumber = "ORD-001235",
                CustomerName = "Fatma Kaya",
                CustomerEmail = "fatma.kaya@email.com",
                ItemCount = 1,
                Total = 890.00m,
                Status = "Tamamlandı",
                StatusColor = "success",
                CreatedAt = DateTime.Now.AddHours(-4)
            },
            new OrderListItem
            {
                Id = 3,
                OrderNumber = "ORD-001236",
                CustomerName = "Mehmet Demir",
                CustomerEmail = "mehmet.demir@email.com",
                ItemCount = 5,
                Total = 5670.00m,
                Status = "İşleniyor",
                StatusColor = "primary",
                CreatedAt = DateTime.Now.AddHours(-6)
            },
            new OrderListItem
            {
                Id = 4,
                OrderNumber = "ORD-001237",
                CustomerName = "Ayşe Öztürk",
                CustomerEmail = "ayse.ozturk@email.com",
                ItemCount = 2,
                Total = 1250.00m,
                Status = "İptal",
                StatusColor = "danger",
                CreatedAt = DateTime.Now.AddDays(-1)
            },
            new OrderListItem
            {
                Id = 5,
                OrderNumber = "ORD-001238",
                CustomerName = "Ali Şahin",
                CustomerEmail = "ali.sahin@email.com",
                ItemCount = 4,
                Total = 3200.00m,
                Status = "Kargoda",
                StatusColor = "info",
                CreatedAt = DateTime.Now.AddDays(-1)
            },
            new OrderListItem
            {
                Id = 6,
                OrderNumber = "ORD-001239",
                CustomerName = "Zeynep Arslan",
                CustomerEmail = "zeynep.arslan@email.com",
                ItemCount = 1,
                Total = 750.00m,
                Status = "Bekleyen",
                StatusColor = "warning",
                CreatedAt = DateTime.Now.AddDays(-2)
            }
        };
    }
}

public sealed class OrderStats
{
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int CancelledOrders { get; set; }
}

public sealed class OrderListItem
{
    public long Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusColor { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
