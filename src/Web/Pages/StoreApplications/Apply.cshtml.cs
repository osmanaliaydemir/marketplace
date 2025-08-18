using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Services;

namespace Web.Pages.StoreApplications;

public class ApplyModel : PageModel
{
    private readonly ApiClient _api;

    public ApplyModel(ApiClient api)
    {
        _api = api;
    }

    [BindProperty]
    public StoreApplicationForm Form { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var dto = await _api.PostAsync<StoreApplicationForm, StoreApplicationDetail>("/api/store-applications", Form);
        TempData["Success"] = $"Başvuru alındı. ID: {dto?.Id}";
        return RedirectToPage("/Index");
    }
}

public sealed class StoreApplicationForm
{
    public string StoreName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ContactEmail { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
    public string? BusinessAddress { get; set; }
    public string? TaxNumber { get; set; }
}

public sealed class StoreApplicationDetail
{
    public long Id { get; set; }
}


