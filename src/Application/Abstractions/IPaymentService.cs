using Application.DTOs.Payments;
using Application.DTOs.Orders;

namespace Application.Abstractions;

public interface IPaymentService
{
    // Payment Processing
    Task<PaymentInitiationResult> InitiatePaymentAsync(PaymentInitiationRequest request);
    Task<PaymentStatusResult> ProcessPaymentAsync(string paymentId, Application.DTOs.Payments.PaymentProcessRequest request);
    Task<PaymentStatusResult> GetPaymentStatusAsync(string paymentId);
    
    // PayTR Integration
    Task<string> CreatePaytrTokenAsync(PaytrTokenRequest request);
    Task<bool> VerifyPaytrCallbackAsync(PaytrCallbackRequest request);
    Task<bool> ProcessPaytrWebhookAsync(PaytrWebhookRequest request);
    
    // Refund and Cancellation
    Task<RefundResult> ProcessRefundAsync(string paymentId, Application.DTOs.Payments.RefundRequest request);
    Task<bool> CancelPaymentAsync(string paymentId, string reason);
    Task<RefundStatusResult> GetRefundStatusAsync(string refundId);
    
    // Payment Splits (Marketplace Commission)
    Task<bool> ProcessPaymentSplitAsync(string paymentId, PaymentSplitRequest request);
    Task<PaymentSplitResult> GetPaymentSplitAsync(string paymentId);
    Task<bool> ReleaseCommissionAsync(string paymentId, long storeId);
    
    // Payment Methods
    Task<IEnumerable<PaymentMethodDto>> GetAvailablePaymentMethodsAsync();
    Task<PaymentMethodDto?> GetPaymentMethodAsync(long methodId);
    Task<bool> ValidatePaymentMethodAsync(PaymentMethodValidationRequest request);
    
    // Security and Validation
    Task<bool> ValidatePaymentRequestAsync(PaymentValidationRequest request);
    Task<bool> VerifySignatureAsync(string signature, string payload);
    Task<bool> CheckFraudRiskAsync(PaymentFraudCheckRequest request);
    
    // Reporting and Analytics
    Task<PaymentStatsDto> GetPaymentStatsAsync(DateTime from, DateTime to);
    Task<PaymentStatsDto> GetStorePaymentStatsAsync(long storeId, DateTime from, DateTime to);
    Task<IEnumerable<PaymentDto>> GetFailedPaymentsAsync(DateTime from, DateTime to);
}
