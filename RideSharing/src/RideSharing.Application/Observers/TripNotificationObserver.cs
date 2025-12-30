using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;
using RideSharing.Domain.Interfaces;

namespace RideSharing.Application.Observers;

/// <summary>
/// Observer that sends notifications for trip events
/// </summary>
public class TripNotificationObserver : INotificationObserver
{
    private readonly INotificationService _notificationService;

    public string ObserverName => "Trip Notification Observer";

    public TripNotificationObserver(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async void OnTripEvent(Trip trip, string eventType)
    {
        switch (eventType)
        {
            case "TripRequested":
                // Notify rider
                await _notificationService.SendNotificationAsync(
                    trip.Rider,
                    NotificationType.TripUpdate,
                    "Trip Requested",
                    "Your trip request has been received. Finding a driver...");
                break;

            case "DriverAssigned":
                // Notify both rider and driver
                if (trip.Driver != null)
                {
                    await _notificationService.SendNotificationAsync(
                        trip.Rider,
                        NotificationType.TripUpdate,
                        "Driver Found!",
                        $"Driver {trip.Driver.Name} is on the way!");

                    await _notificationService.SendNotificationAsync(
                        trip.Driver,
                        NotificationType.TripUpdate,
                        "New Trip Request",
                        $"New ride to {trip.DropoffLocation}");
                }
                break;

            case "TripStarted":
                await _notificationService.SendNotificationAsync(
                    trip.Rider,
                    NotificationType.TripUpdate,
                    "Trip Started",
                    "Your ride has started. Enjoy your journey!");
                break;

            case "TripCompleted":
                await _notificationService.SendNotificationAsync(
                    trip.Rider,
                    NotificationType.TripUpdate,
                    "Trip Completed",
                    $"Your trip is complete. Fare: {trip.ActualFare}. Please rate your driver!");
                
                if (trip.Driver != null)
                {
                    await _notificationService.SendNotificationAsync(
                        trip.Driver,
                        NotificationType.TripUpdate,
                        "Trip Completed",
                        "Trip completed successfully. Please rate your rider!");
                }
                break;

            case "TripCancelled":
                await _notificationService.SendNotificationAsync(
                    trip.Rider,
                    NotificationType.TripUpdate,
                    "Trip Cancelled",
                    "Your trip has been cancelled.");
                
                if (trip.Driver != null)
                {
                    await _notificationService.SendNotificationAsync(
                        trip.Driver,
                        NotificationType.TripUpdate,
                        "Trip Cancelled",
                        "The trip has been cancelled.");
                }
                break;
        }
    }

    public async void OnPaymentEvent(Payment payment, string eventType)
    {
        var trip = payment.Trip;
        
        switch (eventType)
        {
            case "PaymentSuccess":
                await _notificationService.SendNotificationAsync(
                    trip.Rider,
                    NotificationType.PaymentUpdate,
                    "Payment Successful",
                    $"Payment of {payment.Amount} processed successfully!");
                break;

            case "PaymentFailed":
                await _notificationService.SendNotificationAsync(
                    trip.Rider,
                    NotificationType.PaymentUpdate,
                    "Payment Failed",
                    $"Payment failed: {payment.FailureReason}. Please try again.");
                break;

            case "PaymentRefunded":
                await _notificationService.SendNotificationAsync(
                    trip.Rider,
                    NotificationType.PaymentUpdate,
                    "Refund Processed",
                    $"Refund of {payment.Amount} has been processed.");
                break;
        }
    }
}
