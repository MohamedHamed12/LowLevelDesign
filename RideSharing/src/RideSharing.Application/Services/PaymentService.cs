using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;
using RideSharing.Domain.Interfaces;
using RideSharing.Domain.ValueObjects;
using RideSharing.Domain.Exceptions;

namespace RideSharing.Application.Services;

/// <summary>
/// Service for handling payments
/// </summary>
public class PaymentService
{
    private readonly IPaymentGateway _paymentGateway;
    private readonly List<Payment> _payments; // In real app, repository

    public PaymentService(IPaymentGateway paymentGateway)
    {
        _paymentGateway = paymentGateway;
        _payments = new List<Payment>();
    }

    public async Task<Payment> ProcessPaymentAsync(Trip trip, PaymentMethod method, CancellationToken cancellationToken = default)
    {
        if (trip.Status != TripStatus.Completed)
            throw new InvalidOperationException("Can only process payment for completed trips");

        if (trip.ActualFare == null)
            throw new InvalidOperationException("Actual fare not calculated");

        var payment = new Payment(trip, trip.ActualFare, method);
        _payments.Add(payment);

        try
        {
            payment.MarkProcessing();
            
            var result = await _paymentGateway.ProcessPaymentAsync(payment, cancellationToken);
            
            if (result.Success)
            {
                payment.MarkSuccess(result.TransactionId);
                
                // Update driver earnings
                if (trip.Driver != null)
                {
                    var driverShare = trip.ActualFare * 0.80m; // Driver gets 80%
                    trip.Driver.AddEarnings(driverShare);
                }
            }
            else
            {
                payment.MarkFailed(result.ErrorMessage ?? "Payment failed");
            }

            return payment;
        }
        catch (Exception ex)
        {
            payment.MarkFailed(ex.Message);
            throw new PaymentFailedException($"Payment processing failed: {ex.Message}", ex);
        }
    }

    public async Task<bool> RefundPaymentAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        if (payment.Status != PaymentStatus.Success)
            throw new InvalidOperationException("Can only refund successful payments");

        if (string.IsNullOrEmpty(payment.TransactionId))
            throw new InvalidOperationException("No transaction ID found");

        var success = await _paymentGateway.RefundPaymentAsync(payment.TransactionId, payment.Amount, cancellationToken);
        
        if (success)
        {
            payment.Refund();
        }

        return success;
    }

    public async Task<bool> RetryFailedPaymentAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        if (!payment.CanRetry())
            throw new InvalidOperationException("Payment cannot be retried");

        payment.MarkProcessing();
        
        var result = await _paymentGateway.ProcessPaymentAsync(payment, cancellationToken);
        
        if (result.Success)
        {
            payment.MarkSuccess(result.TransactionId);
            return true;
        }
        else
        {
            payment.MarkFailed(result.ErrorMessage ?? "Payment retry failed");
            return false;
        }
    }
}
