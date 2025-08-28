using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Customer.Orders;

[Authorize(Roles = "Customer")]
public sealed class DetailsModel : PageModel
{
    public void OnGet()
    {
    }
}
