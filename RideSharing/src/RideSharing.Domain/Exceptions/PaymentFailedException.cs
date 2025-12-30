namespace RideSharing.Domain.Exceptions;

/// <summary>
/// Exception thrown when a payment transaction fails
/// </summary>
public class PaymentFailedException : Exception
{
    public Guid PaymentId { get; }
    public decimal Amount { get; }
    public string Reason { get; }

    public PaymentFailedException(Guid paymentId, decimal amount, string reason)
        : base($"Payment {paymentId} for amount {amount} failed: {reason}")
    {
        PaymentId = paymentId;
        Amount = amount;
        Reason = reason;
    }

    public PaymentFailedException(string message) : base(message)
    {
        PaymentId = Guid.Empty;
        Amount = 0;
        Reason = string.Empty;
    }

    public PaymentFailedException(string message, Exception innerException)
        : base(message, innerException)
    {
        PaymentId = Guid.Empty;
        Amount = 0;
        Reason = string.Empty;
    }
}
