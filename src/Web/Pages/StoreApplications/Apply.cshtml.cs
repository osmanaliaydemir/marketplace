using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Web.Services;

namespace Web.Pages.StoreApplications
{
    [IgnoreAntiforgeryToken] // TEST: AJAX için anti-forgery doğrulamasını geçici olarak devre dışı bırak
    public class ApplyModel : PageModel
    {
        private readonly ApiClient _apiClient;

        public ApplyModel(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [BindProperty]
        public StoreApplicationViewModel Application { get; set; } = new();

        public void OnGet()
        {
            // Sayfa yüklendiğinde yapılacak işlemler
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Geçersiz form verisi" });
            }

            try
            {
                // API'ye başvuru gönder
                var result = await SubmitApplicationToApi(Application);
                
                if (result.IsSuccess)
                {
                    // AJAX için JSON dön
                    return new JsonResult(new { success = true, message = "Başvuru alındı" });
                }
                else
                {
                    return BadRequest(new { success = false, message = result.ErrorMessage ?? "Başvuru gönderilemedi" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Sunucu hatası", error = ex.Message });
            }
        }

        private async Task<ApiResult> SubmitApplicationToApi(StoreApplicationViewModel application)
        {
            try
            {
                // ApiClient ile başvuruyu gönder
                var response = await _apiClient.PostAsync<StoreApplicationViewModel, object>("/api/storeapplications", application);
                
                if (response != null)
                {
                    return new ApiResult { IsSuccess = true };
                }
                else
                {
                    return new ApiResult { IsSuccess = false, ErrorMessage = "API'den yanıt alınamadı" };
                }
            }
            catch (Exception ex)
            {
                return new ApiResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }
    }

    public class StoreApplicationViewModel
    {
        [Required(ErrorMessage = "İşletme adı gereklidir")]
        [StringLength(100, ErrorMessage = "İşletme adı en fazla 100 karakter olabilir")]
        public string BusinessName { get; set; } = string.Empty;

        [Required(ErrorMessage = "İşletme türü gereklidir")]
        public string BusinessType { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Vergi numarası en fazla 20 karakter olabilir")]
        public string? TaxNumber { get; set; }

        [StringLength(50, ErrorMessage = "İşletme belgesi en fazla 50 karakter olabilir")]
        public string? BusinessLicense { get; set; }

        [Required(ErrorMessage = "İşletme açıklaması gereklidir")]
        [StringLength(500, ErrorMessage = "İşletme açıklaması en fazla 500 karakter olabilir")]
        public string BusinessDescription { get; set; } = string.Empty;

        [Url(ErrorMessage = "Geçerli bir web sitesi URL'si giriniz")]
        public string? Website { get; set; }

        [StringLength(100, ErrorMessage = "Sosyal medya bilgisi en fazla 100 karakter olabilir")]
        public string? SocialMedia { get; set; }

        [Required(ErrorMessage = "Ana kategori gereklidir")]
        public string PrimaryCategory { get; set; } = string.Empty;

        public string? SecondaryCategory { get; set; }

        [Required(ErrorMessage = "Ürün sayısı gereklidir")]
        public int ProductCount { get; set; }

        [Required(ErrorMessage = "Beklenen gelir gereklidir")]
        public decimal ExpectedRevenue { get; set; }

        [Required(ErrorMessage = "E-ticaret deneyimi gereklidir")]
        public string Experience { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Ürün açıklaması en fazla 300 karakter olabilir")]
        public string? ProductDescription { get; set; }

        [Required(ErrorMessage = "İletişim kişisi gereklidir")]
        [StringLength(100, ErrorMessage = "İletişim kişisi en fazla 100 karakter olabilir")]
        public string ContactName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string ContactEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon numarası gereklidir")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şehir gereklidir")]
        public string City { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Kullanım şartlarını kabul etmelisiniz")]
        public bool TermsAccepted { get; set; }

        public bool Newsletter { get; set; }
    }

    public class ApiResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
    }
}


