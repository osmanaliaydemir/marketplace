using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Seller.Products;

[Authorize(Roles = "Seller")]
public sealed class EditModel : PageModel
{
    public void OnGet()
    {
    }
}
