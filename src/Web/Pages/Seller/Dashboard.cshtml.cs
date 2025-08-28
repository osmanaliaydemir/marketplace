using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Seller;

[Authorize(Roles = "Seller")]
public sealed class DashboardModel : PageModel
{
    public void OnGet()
    {
    }
}
