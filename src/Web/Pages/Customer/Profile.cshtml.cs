using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Customer;

[Authorize(Roles = "Customer")]
public sealed class ProfileModel : PageModel
{
    public void OnGet()
    {
    }
}
