using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Services;

namespace Web.Pages.Seller.Orders;

[Authorize(Roles = "Seller")]
public sealed class DetailsModel : PageModel
{
    private readonly ApiClient _api;

    public DetailsModel(ApiClient api)
    {
        _api = api;
    }

    [FromRoute]
    public long Id { get; set; }

    public void OnGet()
    {
        // Page will be populated via JavaScript
    }
}
