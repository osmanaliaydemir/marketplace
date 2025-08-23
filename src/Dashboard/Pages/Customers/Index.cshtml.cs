using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dashboard.Services;

namespace Dashboard.Pages.Customers;

public class IndexModel : PageModel
{
    private readonly ApiClient _api;

    public IndexModel(ApiClient api)
    {
        _api = api;
    }

    public int TotalCustomers { get; set; } = 3250;
    public int ActiveCustomers { get; set; } = 2890;
    public int NewThisMonth { get; set; } = 145;
    public decimal AverageOrderValue { get; set; } = 1850.00m;
    public List<CustomerListItem> Customers { get; set; } = new();

    public async Task OnGetAsync()
    {
        try
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (!string.IsNullOrEmpty(token))
            {
                _api.SetBearer(token);
                
                // API'den müşteri verilerini çek
                var customerStats = await _api.GetAsync<CustomerStats>("/api/dashboard/customer-stats");
                if (customerStats != null)
                {
                    TotalCustomers = customerStats.TotalCustomers;
                    ActiveCustomers = customerStats.ActiveCustomers;
                    NewThisMonth = customerStats.NewThisMonth;
                    AverageOrderValue = customerStats.AverageOrderValue;
                }

                // Müşteri listesini al
                var customers = await _api.GetAsync<IEnumerable<CustomerListItem>>("/api/customers?limit=20");
                if (customers != null)
                {
                    Customers = customers.ToList();
                }
            }
        }
        catch
        {
            // API hatası durumunda örnek veriler kullan
        }

        // Eğer veri yoksa örnek veriler göster
        if (!Customers.Any())
        {
            Customers = GetSampleCustomers();
        }
    }

    private List<CustomerListItem> GetSampleCustomers()
    {
        return new List<CustomerListItem>
        {
            new CustomerListItem
            {
                Id = 1,
                FullName = "Ahmet Yılmaz",
                Email = "ahmet.yilmaz@email.com",
                Phone = "+90 555 123 4567",
                OrderCount = 15,
                TotalSpent = 24500.00m,
                LastOrderDate = DateTime.Now.AddDays(-3),
                Status = "Aktif",
                StatusColor = "success",
                IsVip = true
            },
            new CustomerListItem
            {
                Id = 2,
                FullName = "Fatma Kaya",
                Email = "fatma.kaya@email.com",
                Phone = "+90 555 234 5678",
                OrderCount = 8,
                TotalSpent = 12750.00m,
                LastOrderDate = DateTime.Now.AddDays(-1),
                Status = "Aktif",
                StatusColor = "success",
                IsVip = false
            },
            new CustomerListItem
            {
                Id = 3,
                FullName = "Mehmet Demir",
                Email = "mehmet.demir@email.com",
                Phone = "+90 555 345 6789",
                OrderCount = 22,
                TotalSpent = 45890.00m,
                LastOrderDate = DateTime.Now.AddHours(-6),
                Status = "Aktif",
                StatusColor = "success",
                IsVip = true
            },
            new CustomerListItem
            {
                Id = 4,
                FullName = "Ayşe Öztürk",
                Email = "ayse.ozturk@email.com",
                Phone = "+90 555 456 7890",
                OrderCount = 3,
                TotalSpent = 3250.00m,
                LastOrderDate = DateTime.Now.AddDays(-15),
                Status = "Pasif",
                StatusColor = "warning",
                IsVip = false
            },
            new CustomerListItem
            {
                Id = 5,
                FullName = "Ali Şahin",
                Email = "ali.sahin@email.com",
                Phone = "+90 555 567 8901",
                OrderCount = 12,
                TotalSpent = 18760.00m,
                LastOrderDate = DateTime.Now.AddDays(-2),
                Status = "Aktif",
                StatusColor = "success",
                IsVip = false
            },
            new CustomerListItem
            {
                Id = 6,
                FullName = "Zeynep Arslan",
                Email = "zeynep.arslan@email.com",
                Phone = "+90 555 678 9012",
                OrderCount = 5,
                TotalSpent = 6750.00m,
                LastOrderDate = DateTime.Now.AddDays(-7),
                Status = "Aktif",
                StatusColor = "success",
                IsVip = false
            },
            new CustomerListItem
            {
                Id = 7,
                FullName = "Emre Özkan",
                Email = "emre.ozkan@email.com",
                Phone = "+90 555 789 0123",
                OrderCount = 0,
                TotalSpent = 0.00m,
                LastOrderDate = null,
                Status = "Yeni",
                StatusColor = "info",
                IsVip = false
            },
            new CustomerListItem
            {
                Id = 8,
                FullName = "Selin Yıldız",
                Email = "selin.yildiz@email.com",
                Phone = "+90 555 890 1234",
                OrderCount = 18,
                TotalSpent = 32400.00m,
                LastOrderDate = DateTime.Now.AddDays(-4),
                Status = "Aktif",
                StatusColor = "success",
                IsVip = true
            }
        };
    }
}

public sealed class CustomerStats
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int NewThisMonth { get; set; }
    public decimal AverageOrderValue { get; set; }
}

public sealed class CustomerListItem
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime? LastOrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusColor { get; set; } = string.Empty;
    public bool IsVip { get; set; }
}
