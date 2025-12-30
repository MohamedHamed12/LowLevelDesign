using RideSharing.Domain.Entities;
using RideSharing.Domain.ValueObjects;

namespace RideSharing.Domain.Interfaces;

/// <summary>
/// Interface for payment gateway integration
/// </summary>
public interface IPaymentGateway
{
    Task<(bool Success, string TransactionId, string? ErrorMessage)> ProcessPaymentAsync(
        Payment payment,
        CancellationToken cancellationToken = default);

    Task<bool> RefundPaymentAsync(
        string transactionId,
        Money amount,
        CancellationToken cancellationToken = default);

    Task<bool> VerifyPaymentStatusAsync(
        string transactionId,
        CancellationToken cancellationToken = default);
}
