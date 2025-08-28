using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Services;

namespace Web.Pages.Seller.Reports;

[Authorize(Roles = "Seller")]
public sealed class SalesModel : PageModel
{
    private readonly ApiClient _api;

    public SalesModel(ApiClient api)
    {
        _api = api;
    }

    public void OnGet()
    {
        // Page will be populated via JavaScript
    }
}
