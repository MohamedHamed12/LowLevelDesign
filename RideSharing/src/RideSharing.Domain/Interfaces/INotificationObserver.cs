using RideSharing.Domain.Entities;

namespace RideSharing.Domain.Interfaces;

/// <summary>
/// Defines the contract for notification observers
/// Observer Pattern implementation
/// </summary>
public interface INotificationObserver
{
    /// <summary>
    /// Gets the observer name
    /// </summary>
    string ObserverName { get; }

    /// <summary>
    /// Called when a trip event occurs
    /// </summary>
    void OnTripEvent(Trip trip, string eventType);

    /// <summary>
    /// Called when a payment event occurs
    /// </summary>
    void OnPaymentEvent(Payment payment, string eventType);
}
