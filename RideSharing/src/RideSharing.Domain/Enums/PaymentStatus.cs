namespace RideSharing.Domain.Enums;

/// <summary>
/// Represents the status of a payment transaction
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// Payment is pending
    /// </summary>
    Pending,

    /// <summary>
    /// Payment is being processed
    /// </summary>
    Processing,

    /// <summary>
    /// Payment completed successfully
    /// </summary>
    Success,

    /// <summary>
    /// Payment failed
    /// </summary>
    Failed,

    /// <summary>
    /// Payment was refunded
    /// </summary>
    Refunded,

    /// <summary>
    /// Payment is on hold
    /// </summary>
    OnHold
}
