using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Payments;

public sealed class PaytrAdapter : IPaymentProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PaytrAdapter> _logger;
    private readonly HttpClient _httpClient;

    public PaytrAdapter(IConfiguration configuration, ILogger<PaytrAdapter> logger, HttpClient httpClient)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<string> CreateCheckoutTokenAsync(object ctx, CancellationToken ct = default)
    {
        try
        {
            // PayTR configuration
            var merchantId = _configuration["PayTR:MerchantId"];
            var merchantKey = _configuration["PayTR:MerchantKey"];
            var merchantSalt = _configuration["PayTR:MerchantSalt"];
            var testMode = _configuration.GetValue<bool>("PayTR:TestMode", true);

            if (string.IsNullOrEmpty(merchantId) || string.IsNullOrEmpty(merchantKey) || string.IsNullOrEmpty(merchantSalt))
            {
                _logger.LogError("PayTR configuration is missing");
                throw new InvalidOperationException("PayTR configuration is incomplete");
            }

            // Create payment request
            var paymentRequest = new PaytrPaymentRequest
            {
                MerchantId = merchantId,
                UserIp = GetClientIpAddress(),
                MerchantOid = Guid.NewGuid().ToString("N"),
                Email = "customer@example.com", // Get from context
                PaymentAmount = 1000, // Get from context
                PaytrToken = "",
                UserBasket = "Test Product", // Get from context
                DebugOn = testMode ? 1 : 0,
                NoInstallment = 0,
                MaxInstallment = 0,
                Currency = "TL",
                TestMode = testMode ? 1 : 0,
                Lang = "tr"
            };

            // Generate HMAC signature
            var hashStr = $"{paymentRequest.MerchantId}{paymentRequest.UserIp}{paymentRequest.MerchantOid}{paymentRequest.Email}{paymentRequest.PaymentAmount}{paymentRequest.UserBasket}{paymentRequest.NoInstallment}{paymentRequest.MaxInstallment}{paymentRequest.Currency}{paymentRequest.TestMode}{merchantSalt}";
            var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(hashStr)));
            paymentRequest.PaytrToken = hash;

            // Send request to PayTR
            var response = await SendPaymentRequestAsync(paymentRequest, ct);
            
            if (response.Success)
            {
                _logger.LogInformation("PayTR token created successfully: {Token}", response.Token);
                return response.Token;
            }
            else
            {
                _logger.LogError("PayTR token creation failed: {Reason}", response.Reason);
                throw new InvalidOperationException($"PayTR token creation failed: {response.Reason}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating PayTR checkout token");
            throw;
        }
    }

    public async Task<bool> VerifyCallbackAsync(IHeaderDictionary headers, string rawBody)
    {
        try
        {
            var merchantSalt = _configuration["PayTR:MerchantSalt"];
            if (string.IsNullOrEmpty(merchantSalt))
            {
                _logger.LogError("PayTR merchant salt is missing");
                return false;
            }

            // Verify HMAC signature
            var signature = headers["X-PayTR-Signature"].ToString();
            var expectedSignature = GenerateHmacSignature(rawBody, merchantSalt);

            if (signature != expectedSignature)
            {
                _logger.LogWarning("PayTR callback signature verification failed");
                return false;
            }

            // Parse callback data
            var callbackData = JsonSerializer.Deserialize<PaytrCallbackData>(rawBody);
            if (callbackData == null)
            {
                _logger.LogError("Failed to parse PayTR callback data");
                return false;
            }

            _logger.LogInformation("PayTR callback verified successfully for order: {OrderId}", callbackData.MerchantOid);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying PayTR callback");
            return false;
        }
    }

    private async Task<PaytrResponse> SendPaymentRequestAsync(PaytrPaymentRequest request, CancellationToken ct)
    {
        try
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://www.paytr.com/odeme/api/get-token", content, ct);
            var responseContent = await response.Content.ReadAsStringAsync(ct);

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<PaytrResponse>(responseContent) ?? new PaytrResponse { Success = false };
            }

            _logger.LogError("PayTR API request failed with status: {StatusCode}", response.StatusCode);
            return new PaytrResponse { Success = false, Reason = $"HTTP {response.StatusCode}" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending payment request to PayTR");
            return new PaytrResponse { Success = false, Reason = ex.Message };
        }
    }

    private string GenerateHmacSignature(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToBase64String(hash);
    }

    private string GetClientIpAddress()
    {
        // This should be implemented based on your application context
        return "127.0.0.1";
    }
}

// PayTR Models
public class PaytrPaymentRequest
{
    public string MerchantId { get; set; } = string.Empty;
    public string UserIp { get; set; } = string.Empty;
    public string MerchantOid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int PaymentAmount { get; set; }
    public string PaytrToken { get; set; } = string.Empty;
    public string UserBasket { get; set; } = string.Empty;
    public int DebugOn { get; set; }
    public int NoInstallment { get; set; }
    public int MaxInstallment { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int TestMode { get; set; }
    public string Lang { get; set; } = string.Empty;
}

public class PaytrResponse
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

public class PaytrCallbackData
{
    public string MerchantOid { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TotalAmount { get; set; }
    public string Hash { get; set; } = string.Empty;
}
