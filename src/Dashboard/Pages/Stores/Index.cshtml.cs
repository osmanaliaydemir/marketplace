using Dashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard.Pages.Stores;

public class IndexModel : PageModel
{
    private readonly ApiClient _api;

    public IndexModel(ApiClient api)
    {
        _api = api;
    }

    [BindProperty(SupportsGet = true)]
    public StoreSearchRequest SearchRequest { get; set; } = new();

    public StoreSearchResponse SearchResponse { get; set; } = new();
    public StoreStatsDto Stats { get; set; } = new();

    public async Task OnGetAsync()
    {
        try
        {
            // Session'dan token al
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                // Token yok, varsayılan veriler göster
                Stats = GetSampleStats();
                SearchResponse = GetSampleSearchResponse();
                return;
            }

            _api.SetBearer(token);

            // İstatistikleri al
            var stats = await _api.GetAsync<StoreStatsDto>("/api/stores/stats");
            if (stats != null)
            {
                Stats = stats;
            }

            // Mağaza araması yap
            var searchParams = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(SearchRequest.SearchTerm))
                searchParams["searchTerm"] = SearchRequest.SearchTerm;
            if (SearchRequest.IsActive.HasValue)
                searchParams["isActive"] = SearchRequest.IsActive.Value.ToString();
            if (!string.IsNullOrEmpty(SearchRequest.SortBy))
                searchParams["sortBy"] = SearchRequest.SortBy;
            searchParams["page"] = SearchRequest.Page.ToString();
            searchParams["pageSize"] = SearchRequest.PageSize.ToString();

            var queryString = string.Join("&", searchParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
            var searchUrl = $"/api/stores/search?{queryString}";

            var searchResponse = await _api.GetAsync<StoreSearchResponse>(searchUrl);
            if (searchResponse != null)
            {
                SearchResponse = searchResponse;
            }
        }
        catch (Exception ex)
        {
            // Hata durumunda varsayılan veriler göster
            Stats = GetSampleStats();
            SearchResponse = GetSampleSearchResponse();
        }
    }

    public async Task<IActionResult> OnPostToggleStatusAsync(long id, bool isActive)
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

            var request = new { isActive = isActive };
            await _api.PostAsync<object, object>($"/api/stores/{id}/status", request);

            TempData["Success"] = $"Mağaza durumu {(isActive ? "aktif" : "pasif")} olarak güncellendi.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Mağaza durumu güncellenirken bir hata oluştu.";
        }

        return RedirectToPage();
    }

    private StoreStatsDto GetSampleStats()
    {
        return new StoreStatsDto
        {
            TotalStores = 25,
            ActiveStores = 20,
            InactiveStores = 5,
            TotalSellers = 22,
            ActiveSellers = 18,
            AverageProductsPerStore = 45,
            NewStoresThisMonth = 3,
            NewStoresThisWeek = 1
        };
    }

    private StoreSearchResponse GetSampleSearchResponse()
    {
        return new StoreSearchResponse
        {
            Items = new List<StoreListDto>
            {
                new StoreListDto
                {
                    Id = 1,
                    Name = "TechStore",
                    Slug = "techstore",
                    LogoUrl = null,
                    IsActive = true,
                    SellerName = "Ahmet Yılmaz",
                    ProductCount = 45,
                    CreatedAt = DateTime.Now.AddDays(-30)
                },
                new StoreListDto
                {
                    Id = 2,
                    Name = "Fashion Boutique",
                    Slug = "fashion-boutique",
                    LogoUrl = null,
                    IsActive = true,
                    SellerName = "Ayşe Demir",
                    ProductCount = 32,
                    CreatedAt = DateTime.Now.AddDays(-45)
                },
                new StoreListDto
                {
                    Id = 3,
                    Name = "Home & Garden",
                    Slug = "home-garden",
                    LogoUrl = null,
                    IsActive = false,
                    SellerName = "Mehmet Kaya",
                    ProductCount = 28,
                    CreatedAt = DateTime.Now.AddDays(-60)
                }
            },
            TotalCount = 3,
            Page = 1,
            PageSize = 20,
            TotalPages = 1
        };
    }
}

public sealed class StoreSearchRequest
{
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public long? SellerId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}

public sealed class StoreSearchResponse
{
    public IEnumerable<StoreListDto> Items { get; set; } = Enumerable.Empty<StoreListDto>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public sealed class StoreStatsDto
{
    public int TotalStores { get; set; }
    public int ActiveStores { get; set; }
    public int InactiveStores { get; set; }
    public int TotalSellers { get; set; }
    public int ActiveSellers { get; set; }
    public int AverageProductsPerStore { get; set; }
    public int NewStoresThisMonth { get; set; }
    public int NewStoresThisWeek { get; set; }
}

public sealed class StoreListDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
