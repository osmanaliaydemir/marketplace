using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Seller.Orders;

[Authorize(Roles = "Seller")]
public sealed class DetailsModel : PageModel
{
    public void OnGet()
    {
    }
}
