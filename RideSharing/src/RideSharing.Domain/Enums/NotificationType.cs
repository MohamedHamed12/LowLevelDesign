namespace RideSharing.Domain.Enums;

/// <summary>
/// Represents the type of notification
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// SMS notification
    /// </summary>
    SMS,

    /// <summary>
    /// Email notification
    /// </summary>
    Email,

    /// <summary>
    /// Push notification
    /// </summary>
    Push,

    /// <summary>
    /// In-app notification
    /// </summary>
    InApp,

    /// <summary>
    /// Trip status update notification
    /// </summary>
    TripUpdate,

    /// <summary>
    /// Payment status notification
    /// </summary>
    PaymentUpdate
}
