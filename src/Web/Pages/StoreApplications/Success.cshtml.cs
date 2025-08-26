using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.StoreApplications
{
    public class SuccessModel : PageModel
    {
        public string? SuccessMessage { get; set; }
        
        public void OnGet()
        {
            // TempData'dan başarı mesajını al
            SuccessMessage = TempData["SuccessMessage"]?.ToString();
        }
    }
}
