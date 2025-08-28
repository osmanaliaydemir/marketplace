using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Seller.Products;

[Authorize(Roles = "Seller")]
public sealed class CreateModel : PageModel
{
    public void OnGet()
    {
    }
}
