using Application.Abstractions;
using Application.DTOs.Orders;
using Application.DTOs.Payments;
using Domain.Enums;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public sealed class PaymentService : IPaymentService
{
    private readonly ILogger<PaymentService> _logger;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentProvider _paymentProvider;
    private readonly IOrderRepository _orderRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly ICustomerRepository _customerRepository;

    public PaymentService(
        ILogger<PaymentService> logger,
        IPaymentRepository paymentRepository,
        IPaymentProvider paymentProvider,
        IOrderRepository orderRepository,
        IStoreRepository storeRepository,
        ICustomerRepository customerRepository)
    {
        _logger = logger;
        _paymentRepository = paymentRepository;
        _paymentProvider = paymentProvider;
        _orderRepository = orderRepository;
        _storeRepository = storeRepository;
        _customerRepository = customerRepository;
    }

    public async Task<PaymentInitiationResult> InitiatePaymentAsync(PaymentInitiationRequest request)
    {
        _logger.LogInformation("Initiating payment for order: {OrderId}, Amount: {Amount}", 
            request.OrderId, request.Amount);
        
        try
        {
            // Validate order exists
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order == null)
            {
                _logger.LogWarning("Order not found for payment: {OrderId}", request.OrderId);
                return new PaymentInitiationResult
                {
                    Success = false,
                    ErrorMessage = "Order not found"
                };
            }

            // Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            if (customer == null)
            {
                _logger.LogWarning("Customer not found for payment: {CustomerId}", request.CustomerId);
                return new PaymentInitiationResult
                {
                    Success = false,
                    ErrorMessage = "Customer not found"
                };
            }

            // Validate amount matches order total
            if (request.Amount != order.TotalAmount)
            {
                _logger.LogWarning("Payment amount mismatch: Expected {Expected}, Got {Got}", 
                    order.TotalAmount, request.Amount);
                return new PaymentInitiationResult
                {
                    Success = false,
                    ErrorMessage = "Payment amount mismatch"
                };
            }

            // Create payment record
            var payment = new Payment
            {
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                Amount = request.Amount,
                Currency = request.Currency ?? "TRY",
                PaymentMethod = request.PaymentMethod,
                Status = PaymentStatus.Pending.ToString(),
                CreatedAt = DateTime.UtcNow
            };
            
            var createdPayment = await _paymentRepository.AddAsync(payment);
            _logger.LogInformation("Payment record created: {PaymentId}", createdPayment.Id);
            
            // Initiate payment with provider
            var providerResult = await _paymentProvider.InitiatePaymentAsync(new PaymentRequest
            {
                PaymentId = createdPayment.Id.ToString(),
                Amount = request.Amount,
                Currency = request.Currency ?? "TRY",
                CustomerEmail = customer.User.Email,
                CustomerName = customer.User.FullName ?? "Unknown Customer",
                OrderId = request.OrderId.ToString(),
                Description = $"Order {order.OrderNumber} payment"
            });
            
            if (providerResult.Success)
            {
                // Update payment with provider details
                payment.ProviderPaymentId = providerResult.ProviderPaymentId ?? string.Empty;
                // RedirectUrl is not supported in Payment entity
                payment.Status = PaymentStatus.Initiated.ToString();
                await _paymentRepository.UpdateAsync(payment);
                
                _logger.LogInformation("Payment initiated successfully: {PaymentId}, ProviderId: {ProviderId}", 
                    createdPayment.Id, providerResult.ProviderPaymentId);
                
                return new PaymentInitiationResult
                {
                    Success = true,
                    PaymentId = createdPayment.Id.ToString(),
                    ProviderPaymentId = providerResult.ProviderPaymentId,
                    RedirectUrl = providerResult.RedirectUrl
                };
            }
            else
            {
                // Update payment status to failed
                payment.Status = PaymentStatus.Failed.ToString();
                payment.ErrorMessage = providerResult.ErrorMessage ?? "Unknown error";
                await _paymentRepository.UpdateAsync(payment);
                
                _logger.LogWarning("Payment initiation failed: {PaymentId}, Error: {Error}", 
                    createdPayment.Id, providerResult.ErrorMessage);
                
                return new PaymentInitiationResult
                {
                    Success = false,
                    ErrorMessage = providerResult.ErrorMessage
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating payment for order: {OrderId}", request.OrderId);
            return new PaymentInitiationResult
            {
                Success = false,
                ErrorMessage = "Internal server error"
            };
        }
    }

    public async Task<PaymentStatusResult> ProcessPaymentAsync(string paymentId, Application.DTOs.Payments.PaymentProcessRequest request)
    {
        _logger.LogInformation("Processing payment: {PaymentId}", paymentId);
        
        try
        {
            var payment = await _paymentRepository.GetByProviderPaymentIdAsync(paymentId);
            if (payment == null)
            {
                _logger.LogWarning("Payment not found: {PaymentId}", paymentId);
                return new PaymentStatusResult
                {
                    Success = false,
                    ErrorMessage = "Payment not found"
                };
            }

            // Update payment status
            payment.Status = PaymentStatus.Processing.ToString();
            payment.ProcessedAt = DateTime.UtcNow;
            await _paymentRepository.UpdateAsync(payment);

            // Process with payment provider
            var providerResult = await _paymentProvider.GetPaymentStatusAsync(paymentId);
            
            if (providerResult.Success)
            {
                payment.Status = PaymentStatus.Completed.ToString();
                payment.TransactionId = providerResult.TransactionId ?? string.Empty;
                await _paymentRepository.UpdateAsync(payment);
                
                _logger.LogInformation("Payment processed successfully: {PaymentId}", paymentId);
                return new PaymentStatusResult
                {
                    Success = true,
                    Status = PaymentStatus.Completed.ToString().ToString(),
                    TransactionId = providerResult.TransactionId
                };
            }
            else
            {
                payment.Status = PaymentStatus.Failed.ToString();
                await _paymentRepository.UpdateAsync(payment);
                
                _logger.LogWarning("Payment processing failed: {PaymentId}, Error: {Error}", 
                    paymentId, providerResult.ErrorMessage);
                return new PaymentStatusResult
                {
                    Success = false,
                    ErrorMessage = providerResult.ErrorMessage
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment: {PaymentId}", paymentId);
            throw;
        }
    }

    public async Task<PaymentStatusResult> GetPaymentStatusAsync(string paymentId)
    {
        _logger.LogInformation("Getting payment status: {PaymentId}", paymentId);
        
        try
        {
            var payment = await _paymentRepository.GetByProviderPaymentIdAsync(paymentId);
            if (payment == null)
            {
                _logger.LogWarning("Payment not found: {PaymentId}", paymentId);
                return new PaymentStatusResult
                {
                    Success = false,
                    ErrorMessage = "Payment not found"
                };
            }

            // Get latest status from provider
            var providerResult = await _paymentProvider.GetPaymentStatusAsync(paymentId);
            
            if (providerResult.Success)
            {
                // Update local status if different
                if (payment.Status != providerResult.Status)
                {
                    payment.Status = providerResult.Status;
                    await _paymentRepository.UpdateAsync(payment);
                }
                
                return new PaymentStatusResult
                {
                    Success = true,
                    Status = payment.Status,
                    TransactionId = payment.TransactionId
                };
            }
            else
            {
                return new PaymentStatusResult
                {
                    Success = false,
                    ErrorMessage = providerResult.ErrorMessage
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment status: {PaymentId}", paymentId);
            throw;
        }
    }

    public async Task<string> CreatePaytrTokenAsync(PaytrTokenRequest request)
    {
        _logger.LogInformation("Creating PayTR token for order: {OrderId}", request.OrderId);
        
        try
        {
            // Validate order exists
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order == null)
            {
                _logger.LogWarning("Order not found for PayTR token: {OrderId}", request.OrderId);
                throw new ArgumentException("Order not found");
            }

            // Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(order.CustomerId);
            if (customer == null)
            {
                _logger.LogWarning("Customer not found for PayTR token: {CustomerId}", order.CustomerId);
                throw new ArgumentException("Customer not found");
            }

            // Create payment record if not exists
            var existingPayment = await _paymentRepository.GetByOrderAsync(request.OrderId);
            if (existingPayment == null)
            {
                var payment = new Payment
                {
                    OrderId = request.OrderId,
                    CustomerId = order.CustomerId,
                    Amount = order.TotalAmount,
                    Currency = order.Currency,
                    PaymentMethod = "PayTR",
                    Status = PaymentStatus.Pending.ToString(),
                    CreatedAt = DateTime.UtcNow
                };
                
                await _paymentRepository.AddAsync(payment);
                _logger.LogInformation("Payment record created for PayTR: {PaymentId}", payment.Id);
            }

            // Generate PayTR token (this would typically call PayTR API)
            var token = Guid.NewGuid().ToString("N");
            
            _logger.LogInformation("PayTR token created successfully: {Token}", token);
            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating PayTR token for order: {OrderId}", request.OrderId);
            throw;
        }
    }

    public async Task<bool> VerifyPaytrCallbackAsync(PaytrCallbackRequest request)
    {
        _logger.LogInformation("Verifying PayTR callback for payment: {PaymentId}", request.PaymentId);
        
        try
        {
            // Verify signature (this would typically validate PayTR signature)
            var isValidSignature = await VerifySignatureAsync(request.Signature, request.Payload);
            if (!isValidSignature)
            {
                _logger.LogWarning("Invalid PayTR callback signature for payment: {PaymentId}", request.PaymentId);
                return false;
            }

            // Get payment record
            var payment = await _paymentRepository.GetByProviderPaymentIdAsync(request.PaymentId);
            if (payment == null)
            {
                _logger.LogWarning("Payment not found for PayTR callback: {PaymentId}", request.PaymentId);
                return false;
            }

            // Update payment status based on callback
            if (request.Status == "success")
            {
                payment.Status = PaymentStatus.Completed.ToString();
                payment.TransactionId = request.TransactionId ?? string.Empty;
                payment.ProcessedAt = DateTime.UtcNow;
            }
            else
            {
                payment.Status = PaymentStatus.Failed.ToString();
                payment.ProcessedAt = DateTime.UtcNow;
            }

            await _paymentRepository.UpdateAsync(payment);
            
            _logger.LogInformation("PayTR callback verified successfully for payment: {PaymentId}", payment.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying PayTR callback for payment: {PaymentId}", request.PaymentId);
            return false;
        }
    }

    public async Task<bool> ProcessPaytrWebhookAsync(PaytrWebhookRequest request)
    {
        _logger.LogInformation("Processing PayTR webhook for payment: {PaymentId}", request.PaymentId);
        
        try
        {
            // Verify webhook signature
            var isValidSignature = await VerifySignatureAsync(request.Signature, request.Payload);
            if (!isValidSignature)
            {
                _logger.LogWarning("Invalid PayTR webhook signature for payment: {PaymentId}", request.PaymentId);
                return false;
            }

            // Get payment record
            var payment = await _paymentRepository.GetByProviderPaymentIdAsync(request.PaymentId);
            if (payment == null)
            {
                _logger.LogWarning("Payment not found for PayTR webhook: {PaymentId}", request.PaymentId);
                return false;
            }

            // Process webhook data and update payment
            switch (request.EventType)
            {
                case "payment.completed":
                    payment.Status = PaymentStatus.Completed.ToString();
                    payment.TransactionId = request.TransactionId ?? string.Empty;
                    payment.ProcessedAt = DateTime.UtcNow;
                    break;
                    
                case "payment.failed":
                    payment.Status = PaymentStatus.Failed.ToString();
                    payment.ProcessedAt = DateTime.UtcNow;
                    break;
                    
                case "payment.refunded":
                    payment.Status = PaymentStatus.Refunded.ToString();
                    payment.ProcessedAt = DateTime.UtcNow;
                    break;
                    
                default:
                    _logger.LogInformation("Unhandled PayTR webhook event: {EventType}", request.EventType);
                    break;
            }

            await _paymentRepository.UpdateAsync(payment);
            
            _logger.LogInformation("PayTR webhook processed successfully for payment: {PaymentId}", payment.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing PayTR webhook for payment: {PaymentId}", request.PaymentId);
            return false;
        }
    }

    public async Task<RefundResult> ProcessRefundAsync(string paymentId, Application.DTOs.Payments.RefundRequest request)
    {
        _logger.LogInformation("Processing refund for payment: {PaymentId}, Amount: {Amount}", paymentId, request.Amount);
        
        try
        {
            var payment = await _paymentRepository.GetByProviderPaymentIdAsync(paymentId);
            if (payment == null)
            {
                _logger.LogWarning("Payment not found for refund: {PaymentId}", paymentId);
                return new RefundResult
                {
                    Success = false,
                    ErrorMessage = "Payment not found"
                };
            }

            // Validate refund amount
            if (request.Amount > payment.Amount)
            {
                _logger.LogWarning("Refund amount exceeds payment amount: {RefundAmount} > {PaymentAmount}", 
                    request.Amount, payment.Amount);
                return new RefundResult
                {
                    Success = false,
                    ErrorMessage = "Refund amount exceeds payment amount"
                };
            }

            // Process refund with payment provider
            var providerResult = await _paymentProvider.ProcessRefundAsync(new PaymentRequest
            {
                PaymentId = paymentId,
                Amount = request.Amount,
                Currency = payment.Currency
            });

            if (providerResult.Success)
            {
                // Update payment status
                payment.Status = PaymentStatus.Refunded.ToString();
                await _paymentRepository.UpdateAsync(payment);

                // Create refund record
                var refund = new Refund
                {
                    PaymentId = payment.Id,
                    Amount = request.Amount,
                    Currency = payment.Currency,
                    Reason = request.Reason,
                    Status = RefundStatus.Completed.ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                await _paymentRepository.AddRefundAsync(refund);
                
                _logger.LogInformation("Refund processed successfully: {RefundId}, Amount: {Amount}", 
                    refund.Id, refund.Amount);
                
                return new RefundResult
                {
                    Success = true,
                    RefundId = refund.Id.ToString(),
                    Amount = refund.Amount,
                    Currency = refund.Currency
                };
            }
            else
            {
                _logger.LogWarning("Refund processing failed: {PaymentId}, Error: {Error}", 
                    paymentId, providerResult.ErrorMessage);
                
                return new RefundResult
                {
                    Success = false,
                    ErrorMessage = providerResult.ErrorMessage
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing refund for payment: {PaymentId}", paymentId);
            throw;
        }
    }

    public async Task<bool> CancelPaymentAsync(string paymentId, string reason)
    {
        _logger.LogInformation("Cancelling payment: {PaymentId}, Reason: {Reason}", paymentId, reason);
        
        try
        {
            var payment = await _paymentRepository.GetByProviderPaymentIdAsync(paymentId);
            if (payment == null)
            {
                _logger.LogWarning("Payment not found for cancellation: {PaymentId}", paymentId);
                return false;
            }

            // Check if payment can be cancelled
            if (payment.Status != PaymentStatus.Pending.ToString() && payment.Status != PaymentStatus.Processing.ToString())
            {
                _logger.LogWarning("Payment cannot be cancelled in status: {Status}", payment.Status);
                return false;
            }

            // Update payment status
            payment.Status = PaymentStatus.Cancelled.ToString();
            payment.ProcessedAt = DateTime.UtcNow;
            await _paymentRepository.UpdateAsync(payment);

            _logger.LogInformation("Payment cancelled successfully: {PaymentId}", paymentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling payment: {PaymentId}", paymentId);
            return false;
        }
    }

    public async Task<RefundStatusResult> GetRefundStatusAsync(string refundId)
    {
        _logger.LogInformation("Getting refund status: {RefundId}", refundId);
        
        try
        {
            var refund = await _paymentRepository.GetRefundByIdAsync(refundId);
            if (refund == null)
            {
                _logger.LogWarning("Refund not found: {RefundId}", refundId);
                return new RefundStatusResult
                {
                    Success = false,
                    ErrorMessage = "Refund not found"
                };
            }

            return new RefundStatusResult
            {
                Success = true,
                RefundId = refund.Id.ToString(),
                Status = refund.Status.ToString(),
                Amount = refund.Amount,
                Currency = refund.Currency,
                CreatedAt = refund.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting refund status: {RefundId}", refundId);
            throw;
        }
    }

    public async Task<bool> ProcessPaymentSplitAsync(string paymentId, PaymentSplitRequest request)
    {
        _logger.LogInformation("Processing payment split for payment: {PaymentId}", paymentId);
        
        try
        {
            var payment = await _paymentRepository.GetByProviderPaymentIdAsync(paymentId);
            if (payment == null)
            {
                _logger.LogWarning("Payment not found for split: {PaymentId}", paymentId);
                return false;
            }

            // Validate payment is completed
            if (payment.Status != PaymentStatus.Completed.ToString())
            {
                _logger.LogWarning("Payment must be completed for split: {Status}", payment.Status);
                return false;
            }

            // Calculate commission (typically 5-15% for marketplace)
            var commissionRate = 0.10m; // 10% commission
            var commissionAmount = payment.Amount * commissionRate;
            var storeAmount = payment.Amount - commissionAmount;

            // Create payment split record
            var paymentSplit = new PaymentSplit
            {
                PaymentId = payment.Id,
                StoreId = request.StoreId,
                TotalAmount = payment.Amount,
                CommissionAmount = commissionAmount,
                NetAmount = storeAmount,
                // CommissionRate is not supported in PaymentSplit entity
                Status = PaymentSplitStatus.Pending.ToString(),
                CreatedAt = DateTime.UtcNow
            };

            await _paymentRepository.AddPaymentSplitAsync(paymentSplit);
            
            _logger.LogInformation("Payment split created: {SplitId}, Commission: {Commission}, Store: {Store}", 
                paymentSplit.Id, commissionAmount, storeAmount);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment split for payment: {PaymentId}", paymentId);
            return false;
        }
    }

    public async Task<PaymentSplitResult> GetPaymentSplitAsync(string paymentId)
    {
        _logger.LogInformation("Getting payment split for payment: {PaymentId}", paymentId);
        
        try
        {
            var paymentSplit = await _paymentRepository.GetPaymentSplitByPaymentIdAsync(paymentId);
            if (paymentSplit == null)
            {
                _logger.LogWarning("Payment split not found for payment: {PaymentId}", paymentId);
                return new PaymentSplitResult
                {
                    Success = false,
                    ErrorMessage = "Payment split not found"
                };
            }

            return new PaymentSplitResult
            {
                Success = true,
                PaymentId = paymentId,
                StoreId = paymentSplit.StoreId,
                TotalAmount = paymentSplit.TotalAmount,
                CommissionAmount = paymentSplit.CommissionAmount,
                NetAmount = paymentSplit.NetAmount,
                // CommissionRate is not supported in PaymentSplit entity
                Status = paymentSplit.Status.ToString(),
                CreatedAt = paymentSplit.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment split for payment: {PaymentId}", paymentId);
            throw;
        }
    }

    public async Task<bool> ReleaseCommissionAsync(string paymentId, long storeId)
    {
        _logger.LogInformation("Releasing commission for payment: {PaymentId}, Store: {StoreId}", paymentId, storeId);
        
        try
        {
            var paymentSplit = await _paymentRepository.GetPaymentSplitByPaymentIdAsync(paymentId);
            if (paymentSplit == null)
            {
                _logger.LogWarning("Payment split not found for payment: {PaymentId}", paymentId);
                return false;
            }

            if (paymentSplit.StoreId != storeId)
            {
                _logger.LogWarning("Store ID mismatch for payment split: Expected {Expected}, Got {Got}", 
                    paymentSplit.StoreId, storeId);
                return false;
            }

            // Update payment split status
            paymentSplit.Status = PaymentSplitStatus.Completed.ToString();
                            paymentSplit.ProcessedAt = DateTime.UtcNow;
            await _paymentRepository.UpdatePaymentSplitAsync(paymentSplit);

            _logger.LogInformation("Commission released successfully for payment: {PaymentId}, Store: {StoreId}", 
                paymentId, storeId);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error releasing commission for payment: {PaymentId}, Store: {StoreId}", 
                paymentId, storeId);
            return false;
        }
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetAvailablePaymentMethodsAsync()
    {
        _logger.LogInformation("Getting available payment methods");
        
        try
        {
            // This would typically come from a configuration or database
            var paymentMethods = new List<PaymentMethodDto>
            {
                new PaymentMethodDto
                {
                    Id = 1,
                    Name = "Credit Card",
                    Description = "Visa, MasterCard, American Express",
                    IsActive = true,
                    ProcessingFee = 0.025m, // 2.5%
                    ProcessingFeeType = "Percentage"
                },
                new PaymentMethodDto
                {
                    Id = 2,
                    Name = "PayTR",
                    Description = "Turkish payment gateway",
                    IsActive = true,
                    ProcessingFee = 0.029m, // 2.9%
                    ProcessingFeeType = "Percentage"
                },
                new PaymentMethodDto
                {
                    Id = 3,
                    Name = "Bank Transfer",
                    Description = "Direct bank transfer",
                    IsActive = true,
                    ProcessingFee = 0m,
                    ProcessingFeeType = "Fixed"
                }
            };

            _logger.LogInformation("Found {Count} available payment methods", paymentMethods.Count);
            return paymentMethods;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available payment methods");
            throw;
        }
    }

    public async Task<PaymentMethodDto?> GetPaymentMethodAsync(long methodId)
    {
        _logger.LogInformation("Getting payment method: {MethodId}", methodId);
        
        try
        {
            var paymentMethods = await GetAvailablePaymentMethodsAsync();
            var paymentMethod = paymentMethods.FirstOrDefault(pm => pm.Id == methodId);
            
            if (paymentMethod == null)
            {
                _logger.LogWarning("Payment method not found: {MethodId}", methodId);
                return null;
            }

            _logger.LogInformation("Found payment method: {Name}", paymentMethod.Name);
            return paymentMethod;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment method: {MethodId}", methodId);
            throw;
        }
    }

    public async Task<bool> ValidatePaymentMethodAsync(PaymentMethodValidationRequest request)
    {
        _logger.LogInformation("Validating payment method: {MethodId}", request.MethodId);
        
        try
        {
            var paymentMethod = await GetPaymentMethodAsync(request.MethodId);
            if (paymentMethod == null)
            {
                _logger.LogWarning("Payment method not found for validation: {MethodId}", request.MethodId);
                return false;
            }

            if (!paymentMethod.IsActive)
            {
                _logger.LogWarning("Payment method is not active: {MethodId}", request.MethodId);
                return false;
            }

            // Validate amount limits if specified
            if (request.Amount.HasValue)
            {
                if (request.Amount.Value < 1.0m) // Minimum 1 TL
                {
                    _logger.LogWarning("Payment amount below minimum: {Amount}", request.Amount.Value);
                    return false;
                }

                if (request.Amount.Value > 100000.0m) // Maximum 100,000 TL
                {
                    _logger.LogWarning("Payment amount above maximum: {Amount}", request.Amount.Value);
                    return false;
                }
            }

            _logger.LogInformation("Payment method validation successful: {MethodId}", request.MethodId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating payment method: {MethodId}", request.MethodId);
            return false;
        }
    }

    public async Task<bool> ValidatePaymentRequestAsync(PaymentValidationRequest request)
    {
        _logger.LogInformation("Validating payment request for order: {OrderId}", request.OrderId);
        
        try
        {
            // Validate order exists
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order == null)
            {
                _logger.LogWarning("Order not found for payment validation: {OrderId}", request.OrderId);
                return false;
            }

            // Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(order.CustomerId);
            if (customer == null)
            {
                _logger.LogWarning("Customer not found for payment validation: {CustomerId}", order.CustomerId);
                return false;
            }

            // Validate amount matches order total
            if (request.Amount != order.TotalAmount)
            {
                _logger.LogWarning("Payment amount mismatch: Expected {Expected}, Got {Got}", 
                    order.TotalAmount, request.Amount);
                return false;
            }

            // Validate payment method
            if (!await ValidatePaymentMethodAsync(new PaymentMethodValidationRequest
            {
                MethodId = request.PaymentMethodId,
                Amount = request.Amount
            }))
            {
                return false;
            }

            _logger.LogInformation("Payment request validation successful for order: {OrderId}", request.OrderId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating payment request for order: {OrderId}", request.OrderId);
            return false;
        }
    }

    public async Task<bool> VerifySignatureAsync(string signature, string payload)
    {
        _logger.LogInformation("Verifying signature for payload");
        
        try
        {
            // This is a simplified signature verification
            // In production, you would use proper cryptographic verification
            
            if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(payload))
            {
                _logger.LogWarning("Invalid signature or payload");
                return false;
            }

            // For demo purposes, we'll just check if signature is not empty
            // In real implementation, you would:
            // 1. Hash the payload using the same algorithm as the sender
            // 2. Compare with the provided signature
            // 3. Use proper cryptographic libraries
            
            var isValid = !string.IsNullOrEmpty(signature) && signature.Length >= 32;
            
            _logger.LogInformation("Signature verification result: {IsValid}", isValid);
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying signature");
            return false;
        }
    }

    public async Task<bool> CheckFraudRiskAsync(PaymentFraudCheckRequest request)
    {
        _logger.LogInformation("Checking fraud risk for payment: {PaymentId}", request.PaymentId);
        
        try
        {
            var payment = await _paymentRepository.GetByProviderPaymentIdAsync(request.PaymentId);
            if (payment == null)
            {
                _logger.LogWarning("Payment not found for fraud check: {PaymentId}", request.PaymentId);
                return false;
            }

            var customer = await _customerRepository.GetByIdAsync(payment.CustomerId);
            if (customer == null)
            {
                _logger.LogWarning("Customer not found for fraud check: {CustomerId}", payment.CustomerId);
                return false;
            }

            // Basic fraud risk assessment
            var riskScore = 0;

            // Check customer history
            var customerPayments = await _paymentRepository.GetByCustomerAsync(payment.CustomerId);
            var successfulPayments = customerPayments.Count(p => p.Status == PaymentStatus.Completed.ToString());
            var failedPayments = customerPayments.Count(p => p.Status == PaymentStatus.Failed.ToString());

            if (successfulPayments == 0 && failedPayments > 0)
                riskScore += 30; // New customer with failed payments

            if (failedPayments > successfulPayments)
                riskScore += 20; // More failed than successful payments

            // Check amount patterns
            var avgAmount = customerPayments.Any() ? customerPayments.Average(p => p.Amount) : 0;
            if (payment.Amount > avgAmount * 3 && avgAmount > 0)
                riskScore += 25; // Unusually high amount

            // Check location (if available)
            if (!string.IsNullOrEmpty(request.IpAddress))
            {
                // In production, you would check against known fraud IPs
                // For demo, we'll just log the IP
                _logger.LogInformation("Payment IP address: {IpAddress}", request.IpAddress);
            }

            // Risk threshold
            var isHighRisk = riskScore >= 50;
            
            _logger.LogInformation("Fraud risk assessment for payment {PaymentId}: Score {Score}, High Risk: {IsHighRisk}", 
                payment.Id, riskScore, isHighRisk);

            return !isHighRisk; // Return true if low risk
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking fraud risk for payment: {PaymentId}", request.PaymentId);
            return false;
        }
    }

    public async Task<PaymentStatsDto> GetPaymentStatsAsync(DateTime from, DateTime to)
    {
        _logger.LogInformation("Getting payment stats from {From} to {To}", from, to);
        
        try
        {
            var payments = await _paymentRepository.GetByDateRangeAsync(from, to);
            
            var totalPayments = payments.Count();
            var successfulPayments = payments.Count(p => p.Status == PaymentStatus.Completed.ToString());
            var failedPayments = payments.Count(p => p.Status == PaymentStatus.Failed.ToString());
            var pendingPayments = payments.Count(p => p.Status == PaymentStatus.Pending.ToString());
            var totalAmount = payments.Where(p => p.Status == PaymentStatus.Completed.ToString()).Sum(p => p.Amount);
            var avgAmount = successfulPayments > 0 ? totalAmount / successfulPayments : 0;

            var stats = new PaymentStatsDto
            {
                PeriodFrom = from,
                PeriodTo = to,
                TotalPayments = totalPayments,
                SuccessfulPayments = successfulPayments,
                FailedPayments = failedPayments,
                PendingPayments = pendingPayments,
                TotalAmount = totalAmount,
                AverageAmount = avgAmount,
                SuccessRate = totalPayments > 0 ? (decimal)successfulPayments / totalPayments * 100 : 0
            };

            _logger.LogInformation("Payment stats calculated: Total {Total}, Success {Success}, Amount {Amount}", 
                totalPayments, successfulPayments, totalAmount);
            
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment stats from {From} to {To}", from, to);
            throw;
        }
    }

    public async Task<PaymentStatsDto> GetStorePaymentStatsAsync(long storeId, DateTime from, DateTime to)
    {
        _logger.LogInformation("Getting payment stats for store {StoreId} from {From} to {To}", storeId, from, to);
        
        try
        {
            // Get orders for the store in the date range
            var orders = await _orderRepository.GetByStoreAsync(storeId);
            var storeOrders = orders.Where(o => o.CreatedAt >= from && o.CreatedAt <= to);
            
            // Get payments for these orders
            var orderIds = storeOrders.Select(o => o.Id).ToList();
            var payments = new List<Payment>();
            
            foreach (var orderId in orderIds)
            {
                var orderPayments = await _paymentRepository.GetByOrderAsync(orderId);
                if (orderPayments != null)
                    payments.AddRange(orderPayments);
            }
            
            var totalPayments = payments.Count;
            var successfulPayments = payments.Count(p => p.Status == PaymentStatus.Completed.ToString());
            var failedPayments = payments.Count(p => p.Status == PaymentStatus.Failed.ToString());
            var pendingPayments = payments.Count(p => p.Status == PaymentStatus.Pending.ToString());
            var totalAmount = payments.Where(p => p.Status == PaymentStatus.Completed.ToString()).Sum(p => p.Amount);
            var avgAmount = successfulPayments > 0 ? totalAmount / successfulPayments : 0;

            var stats = new PaymentStatsDto
            {
                PeriodFrom = from,
                PeriodTo = to,
                TotalPayments = totalPayments,
                SuccessfulPayments = successfulPayments,
                FailedPayments = failedPayments,
                PendingPayments = pendingPayments,
                TotalAmount = totalAmount,
                AverageAmount = avgAmount,
                SuccessRate = totalPayments > 0 ? (decimal)successfulPayments / totalPayments * 100 : 0
            };

            _logger.LogInformation("Store payment stats calculated: Store {StoreId}, Total {Total}, Success {Success}, Amount {Amount}", 
                storeId, totalPayments, successfulPayments, totalAmount);
            
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting store payment stats for store {StoreId} from {From} to {To}", storeId, from, to);
            throw;
        }
    }

    public async Task<IEnumerable<PaymentDto>> GetFailedPaymentsAsync(DateTime from, DateTime to)
    {
        _logger.LogInformation("Getting failed payments from {From} to {To}", from, to);
        
        try
        {
            var payments = await _paymentRepository.GetByDateRangeAsync(from, to);
            var failedPayments = payments.Where(p => p.Status == "Failed");
            
            var paymentDtos = await MapToPaymentListDtosAsync(failedPayments);
            
            _logger.LogInformation("Found {Count} failed payments", paymentDtos.Count());
            return paymentDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting failed payments from {From} to {To}", from, to);
            throw;
        }
    }
    
    #region Private Helper Methods
    
    private async Task<PaymentDto> MapToPaymentDtoAsync(Payment payment)
    {
        var order = await _orderRepository.GetByIdAsync(payment.OrderId);
        var customer = order != null ? await _customerRepository.GetByIdAsync(order.CustomerId) : null;
        var store = order != null ? await _storeRepository.GetByIdAsync(order.StoreId) : null;
        
        return new PaymentDto
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            CustomerId = customer?.Id ?? 0,
            CustomerName = customer != null ? customer.User.FullName ?? "Unknown" : "Unknown",
            StoreId = store?.Id ?? 0,
            StoreName = store?.Name ?? "Unknown",
            Amount = payment.Amount,
            Currency = payment.Currency,
            Status = payment.Status,
            PaymentMethod = payment.PaymentMethod,
            ProviderPaymentId = payment.ProviderPaymentId,
            TransactionId = payment.TransactionId,
            CreatedAt = payment.CreatedAt,
            ProcessedAt = payment.ProcessedAt
        };
    }
    
    private async Task<IEnumerable<PaymentDto>> MapToPaymentListDtosAsync(IEnumerable<Payment> payments)
    {
        var paymentDtos = new List<PaymentDto>();
        
        foreach (var payment in payments)
        {
            paymentDtos.Add(await MapToPaymentDtoAsync(payment));
        }
        
        return paymentDtos;
    }
    
         #endregion
 }
