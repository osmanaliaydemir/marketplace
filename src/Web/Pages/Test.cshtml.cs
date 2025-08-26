using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages
{
    public class TestModel : PageModel
    {
        [BindProperty]
        public string TestInput { get; set; } = string.Empty;
        
        public string? Message { get; set; }

        public void OnGet()
        {
            // Sayfa yüklendiğinde yapılacak işlemler
        }

        public IActionResult OnPost()
        {
            // Basit POST testi
            Message = $"POST başarılı! Gönderilen veri: {TestInput}";
            
            // JSON response döndür
            return new JsonResult(new { 
                success = true, 
                message = Message,
                data = TestInput,
                timestamp = DateTime.Now
            });
        }
    }
}
