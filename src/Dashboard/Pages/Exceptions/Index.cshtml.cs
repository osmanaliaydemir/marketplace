using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Dashboard.Services;

namespace Dashboard.Pages.Exceptions;

public class IndexModel : PageModel
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ApiClient apiClient, ILogger<IndexModel> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public DateTime StartDate { get; set; } = DateTime.UtcNow.AddDays(-7);

    [BindProperty(SupportsGet = true)]
    public DateTime EndDate { get; set; } = DateTime.UtcNow;

    [BindProperty(SupportsGet = true)]
    public string? SeverityFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? StatusFilter { get; set; }

    public List<ExceptionLogItem> Exceptions { get; set; } = new();
    public ExceptionAnalytics? Analytics { get; set; }
    public List<ExceptionTrend> Trends { get; set; } = new();
    public int TotalExceptions { get; set; }
    public int UnresolvedExceptions { get; set; }
    public int CriticalExceptions { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            await LoadDataAsync();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading exception dashboard");
            TempData["Error"] = "Exception dashboard yüklenirken hata oluştu";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostResolveAsync(long id)
    {
        try
        {
            var request = new { ResolvedBy = User.Identity?.Name ?? "Unknown", Notes = "Dashboard'dan çözüldü" };
            await _apiClient.PutAsync<object, object>($"api/ExceptionLog/{id}/resolve", request);
            
            TempData["Success"] = "Exception başarıyla çözüldü olarak işaretlendi";
            await LoadDataAsync();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving exception {ExceptionId}", id);
            TempData["Error"] = "Exception çözülürken hata oluştu";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostIgnoreAsync(long id)
    {
        try
        {
            var request = new { IgnoredBy = User.Identity?.Name ?? "Unknown", Notes = "Dashboard'dan göz ardı edildi" };
            await _apiClient.PutAsync<object, object>($"api/ExceptionLog/{id}/ignore", request);
            
            TempData["Success"] = "Exception başarıyla göz ardı edildi olarak işaretlendi";
            await LoadDataAsync();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ignoring exception {ExceptionId}", id);
            TempData["Error"] = "Exception göz ardı edilirken hata oluştu";
            return Page();
        }
    }

    private async Task LoadDataAsync()
    {
        // Analytics yükle
        var analyticsResponse = await _apiClient.GetAsync<ExceptionAnalytics>(
            $"api/ExceptionLog/analytics?startDate={StartDate:yyyy-MM-dd}&endDate={EndDate:yyyy-MM-dd}");
        
        if (analyticsResponse != null)
        {
            Analytics = analyticsResponse;
            TotalExceptions = Analytics.TotalExceptions;
            UnresolvedExceptions = Analytics.UnresolvedExceptions;
            CriticalExceptions = Analytics.CriticalExceptions;
        }

        // Trends yükle
        var trendsResponse = await _apiClient.GetAsync<List<ExceptionTrend>>(
            $"api/ExceptionLog/trends?startDate={StartDate:yyyy-MM-dd}&endDate={EndDate:yyyy-MM-dd}&groupBy=day");
        
        if (trendsResponse != null)
        {
            Trends = trendsResponse;
        }

        // Exceptions yükle
        var exceptionsResponse = await _apiClient.GetAsync<List<ExceptionLogItem>>(
            $"api/ExceptionLog?count=100");
        
        if (exceptionsResponse != null)
        {
            Exceptions = exceptionsResponse;
        }

        // Filtreleme uygula
        if (!string.IsNullOrEmpty(SeverityFilter))
        {
            Exceptions = Exceptions.Where(e => e.Severity.ToString() == SeverityFilter).ToList();
        }

        if (!string.IsNullOrEmpty(StatusFilter))
        {
            Exceptions = Exceptions.Where(e => e.Status.ToString() == StatusFilter).ToList();
        }
    }
}

public sealed class ExceptionLogItem
{
    public long Id { get; set; }
    public string ExceptionType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public string? RequestPath { get; set; }
    public string? RequestMethod { get; set; }
    public string? IpAddress { get; set; }
    public string? UserId { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public int OccurrenceCount { get; set; }
    public DateTime LastOccurrence { get; set; }
}

public sealed class ExceptionAnalytics
{
    public int TotalExceptions { get; set; }
    public int UnresolvedExceptions { get; set; }
    public int CriticalExceptions { get; set; }
    public int HighSeverityExceptions { get; set; }
    public Dictionary<string, int> ExceptionTypeDistribution { get; set; } = new();
    public Dictionary<string, int> SeverityDistribution { get; set; } = new();
    public Dictionary<string, int> StatusDistribution { get; set; } = new();
    public double AverageResolutionTimeHours { get; set; }
    public List<string> TopExceptionTypes { get; set; } = new();
    public List<string> TopAffectedEndpoints { get; set; } = new();
}

public sealed class ExceptionTrend
{
    public DateTime Date { get; set; }
    public string Group { get; set; } = string.Empty;
    public int Count { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string ExceptionType { get; set; } = string.Empty;
}
