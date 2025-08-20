namespace Application.Abstractions;

public interface IPaymentProvider
{
    Task<Application.DTOs.Payments.PaymentInitiationResult> InitiatePaymentAsync(Application.DTOs.Payments.PaymentRequest request);
    Task<Application.DTOs.Payments.PaymentStatusResult> GetPaymentStatusAsync(string paymentId);
    Task<bool> VerifyCallbackAsync(Application.DTOs.Payments.PaymentCallbackRequest request);
    Task<Application.DTOs.Payments.RefundResult> ProcessRefundAsync(Application.DTOs.Payments.PaymentRequest request);
}
