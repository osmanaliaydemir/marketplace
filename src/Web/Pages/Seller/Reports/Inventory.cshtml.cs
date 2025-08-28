using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Seller.Reports;

[Authorize(Roles = "Seller")]
public sealed class InventoryModel : PageModel
{
    public void OnGet()
    {
    }
}
