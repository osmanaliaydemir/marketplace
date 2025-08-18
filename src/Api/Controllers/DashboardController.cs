using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Persistence.Repositories;

namespace Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = "Admin")]
public sealed class DashboardController : ControllerBase
{
    private readonly IStoreUnitOfWork _unitOfWork;

    public DashboardController(IStoreUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStats>> GetStats()
    {
        try
        {
            var stores = await _unitOfWork.Stores.GetAllAsync();
            var sellers = await _unitOfWork.Sellers.GetAllAsync();
            var storeApplications = await _unitOfWork.StoreApplications.GetAllAsync();
            var orders = await _unitOfWork.Orders.GetAllAsync();

            var stats = new DashboardStats
            {
                TotalSales = orders.Where(o => o.Status == Domain.Entities.OrderStatus.Completed)
                                 .Sum(o => o.TotalAmount),
                TotalOrders = orders.Count(),
                PendingApplications = storeApplications.Count(a => a.Status == Domain.Entities.StoreApplicationStatus.Pending),
                ActiveStores = stores.Count(s => s.IsActive)
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Dashboard istatistikleri alınırken bir hata oluştu", Error = ex.Message });
        }
    }

    [HttpGet("recent-activities")]
    public async Task<ActionResult<IEnumerable<ActivityItem>>> GetRecentActivities()
    {
        try
        {
            var activities = new List<ActivityItem>();

            // Son mağaza başvuruları
            var recentApplications = (await _unitOfWork.StoreApplications.GetAllAsync())
                .OrderByDescending(a => a.CreatedAt)
                .Take(5);

            foreach (var app in recentApplications)
            {
                activities.Add(new ActivityItem
                {
                    Title = "Yeni Mağaza Başvurusu",
                    Description = $"{app.StoreName} mağazası başvuru yaptı",
                    Icon = "bi bi-shop",
                    TimeAgo = GetTimeAgo(app.CreatedAt)
                });
            }

            // Son siparişler
            var recentOrders = (await _unitOfWork.Orders.GetAllAsync())
                .OrderByDescending(o => o.CreatedAt)
                .Take(3);

            foreach (var order in recentOrders)
            {
                activities.Add(new ActivityItem
                {
                    Title = "Yeni Sipariş",
                    Description = $"#{order.Id} siparişi oluşturuldu - ₺{order.TotalAmount:N2}",
                    Icon = "bi bi-cart",
                    TimeAgo = GetTimeAgo(order.CreatedAt)
                });
            }

            // Son mağaza güncellemeleri
            var recentStores = (await _unitOfWork.Stores.GetAllAsync())
                .Where(s => s.ModifiedAt.HasValue)
                .OrderByDescending(s => s.ModifiedAt)
                .Take(2);

            foreach (var store in recentStores)
            {
                activities.Add(new ActivityItem
                {
                    Title = "Mağaza Güncellendi",
                    Description = $"{store.Name} mağazası güncellendi",
                    Icon = "bi bi-shop",
                    TimeAgo = GetTimeAgo(store.ModifiedAt!.Value)
                });
            }

            return Ok(activities.OrderByDescending(a => a.TimeAgo).Take(10));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Son aktiviteler alınırken bir hata oluştu", Error = ex.Message });
        }
    }

    private string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalDays >= 1)
        {
            var days = (int)timeSpan.TotalDays;
            return $"{days} gün önce";
        }
        else if (timeSpan.TotalHours >= 1)
        {
            var hours = (int)timeSpan.TotalHours;
            return $"{hours} saat önce";
        }
        else if (timeSpan.TotalMinutes >= 1)
        {
            var minutes = (int)timeSpan.TotalMinutes;
            return $"{minutes} dakika önce";
        }
        else
        {
            return "Az önce";
        }
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
