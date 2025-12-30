using RideSharing.Domain.Entities;
using RideSharing.Domain.Interfaces;
using RideSharing.Domain.ValueObjects;

namespace RideSharing.Infrastructure.Services;

/// <summary>
/// Mock payment gateway for testing
/// In production, this would integrate with Stripe, PayPal, etc.
/// </summary>
public class MockPaymentGateway : IPaymentGateway
{
    private readonly Random _random = new();
    private readonly Dictionary<string, bool> _transactions = new();

    public async Task<(bool Success, string TransactionId, string? ErrorMessage)> ProcessPaymentAsync(
        Payment payment,
        CancellationToken cancellationToken = default)
    {
        // Simulate network delay
        await Task.Delay(500, cancellationToken);

        // 95% success rate
        var success = _random.Next(100) < 95;

        if (success)
        {
            var transactionId = $"TXN-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
            _transactions[transactionId] = true;
            return (true, transactionId, null);
        }
        else
        {
            return (false, string.Empty, "Insufficient funds or card declined");
        }
    }

    public async Task<bool> RefundPaymentAsync(
        string transactionId,
        Money amount,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(300, cancellationToken);

        if (_transactions.ContainsKey(transactionId))
        {
            _transactions[transactionId] = false; // Mark as refunded
            return true;
        }

        return false;
    }

    public async Task<bool> VerifyPaymentStatusAsync(
        string transactionId,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(100, cancellationToken);
        return _transactions.TryGetValue(transactionId, out var isActive) && isActive;
    }
}
