using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Services;

namespace Web.Pages.Seller.Store;

[Authorize(Roles = "Seller")]
public sealed class IndexModel : PageModel
{
    private readonly ApiClient _api;

    public IndexModel(ApiClient api)
    {
        _api = api;
    }

    public void OnGet()
    {
        // Page will be populated via JavaScript
    }
}
