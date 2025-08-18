using Dashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard.Pages.StoreApplications;

public class IndexModel : PageModel
{
    private readonly ApiClient _api;

    public IndexModel(ApiClient api)
    {
        _api = api;
    }

    public List<StoreApplicationListItem> Applications { get; set; } = new();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            // Session'dan token al
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                // Token yok, varsayılan veriler göster
                Applications = GetSampleApplications();
                return;
            }

            _api.SetBearer(token);

            // Başvuruları al
            var response = await _api.GetAsync<IEnumerable<StoreApplicationListItem>>("/api/store-applications");
            if (response != null)
            {
                Applications = response.ToList();
            }
        }
        catch
        {
            // Hata durumunda varsayılan veriler göster
            Applications = GetSampleApplications();
        }

        // TempData mesajlarını al
        if (TempData["Success"] != null)
            SuccessMessage = TempData["Success"].ToString();
        if (TempData["Error"] != null)
            ErrorMessage = TempData["Error"].ToString();
    }

    public async Task<IActionResult> OnPostApproveAsync(long id)
    {
        try
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Oturum süresi dolmuş. Lütfen tekrar giriş yapın.";
                return RedirectToPage("/Login");
            }

            _api.SetBearer(token);

            await _api.PostAsync<object, object>($"/api/store-applications/{id}/approve", new { });
            TempData["Success"] = "Mağaza başvurusu onaylandı.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Başvuru onaylanırken bir hata oluştu.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(long id, string reason)
    {
        try
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Oturum süresi dolmuş. Lütfen tekrar giriş yapın.";
                return RedirectToPage("/Login");
            }

            _api.SetBearer(token);

            await _api.PostAsync<object, object>($"/api/store-applications/{id}/reject", new { reason });
            TempData["Success"] = "Mağaza başvurusu reddedildi.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Başvuru reddedilirken bir hata oluştu.";
        }

        return RedirectToPage();
    }

    private List<StoreApplicationListItem> GetSampleApplications()
    {
        return new List<StoreApplicationListItem>
        {
            new StoreApplicationListItem
            {
                Id = 1,
                StoreName = "TechStore",
                Slug = "techstore",
                ContactEmail = "info@techstore.com",
                ContactPhone = "+90 555 123 4567",
                Status = StoreApplicationStatus.Pending,
                CreatedAt = DateTime.Now.AddDays(-2)
            },
            new StoreApplicationListItem
            {
                Id = 2,
                StoreName = "Fashion Boutique",
                Slug = "fashion-boutique",
                ContactEmail = "hello@fashionboutique.com",
                ContactPhone = "+90 555 987 6543",
                Status = StoreApplicationStatus.Approved,
                CreatedAt = DateTime.Now.AddDays(-5)
            }
        };
    }
}

public sealed class StoreApplicationListItem
{
    public long Id { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
    public StoreApplicationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum StoreApplicationStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Cancelled = 4
}


