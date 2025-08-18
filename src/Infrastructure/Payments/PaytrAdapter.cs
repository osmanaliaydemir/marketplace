using Microsoft.AspNetCore.Http;

namespace Infrastructure.Payments;

public sealed class PaytrAdapter : IPaymentProvider
{
    public Task<string> CreateCheckoutTokenAsync(object ctx, CancellationToken ct = default)
        => Task.FromResult("<token>"); // TODO: PayTR dokümantasyonuna göre gerçek implementasyon

    public Task<bool> VerifyCallbackAsync(IHeaderDictionary headers, string rawBody)
        => Task.FromResult(true); // TODO: İmza/HMAC doğrulama
}
