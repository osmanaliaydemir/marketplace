using Microsoft.AspNetCore.Http;

namespace Infrastructure.Payments;

public interface IPaymentProvider
{
    Task<string> CreateCheckoutTokenAsync(object checkoutContext, CancellationToken ct = default);
    Task<bool> VerifyCallbackAsync(IHeaderDictionary headers, string rawBody);
}
