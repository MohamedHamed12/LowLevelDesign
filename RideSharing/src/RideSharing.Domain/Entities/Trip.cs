using RideSharing.Domain.Enums;
using RideSharing.Domain.Interfaces;
using RideSharing.Domain.ValueObjects;
using RideSharing.Domain.Exceptions;

namespace RideSharing.Domain.Entities;

/// <summary>
/// Represents a trip/ride in the system
/// </summary>
public class Trip
{
    private readonly List<INotificationObserver> _observers = new();
    private ITripState? _currentState;

    public Guid Id { get; private set; }
    public Rider Rider { get; private set; }
    public Driver? Driver { get; private set; }
    public Location PickupLocation { get; private set; }
    public Location DropoffLocation { get; private set; }
    public TripStatus Status { get; private set; }
    public VehicleType RequestedVehicleType { get; private set; }
    public Money? EstimatedFare { get; private set; }
    public Money? ActualFare { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public DateTime? PickupTime { get; private set; }
    public DateTime? DropoffTime { get; private set; }
    public double? Distance { get; private set; }
    public int? Duration { get; private set; }
    public string? CancellationReason { get; private set; }

    public string CurrentStateName => _currentState?.StateName ?? "None";

    public Trip(Rider rider, Location pickupLocation, Location dropoffLocation, VehicleType vehicleType)
    {
        Id = Guid.NewGuid();
        Rider = rider ?? throw new ArgumentNullException(nameof(rider));
        PickupLocation = pickupLocation ?? throw new ArgumentNullException(nameof(pickupLocation));
        DropoffLocation = dropoffLocation ?? throw new ArgumentNullException(nameof(dropoffLocation));
        RequestedVehicleType = vehicleType;
        Status = TripStatus.Requested;
        RequestedAt = DateTime.UtcNow;
    }

    public void SetState(ITripState state)
    {
        _currentState = state;
    }

    public void SetEstimatedFare(Money fare)
    {
        EstimatedFare = fare;
    }

    public void AssignDriver(Driver driver)
    {
        if (Status != TripStatus.Requested)
            throw new InvalidTripStateException($"Cannot assign driver in {Status} state");

        if (!driver.IsAvailable)
            throw new InvalidOperationException("Driver is not available");

        if (!driver.CanAcceptVehicleType(RequestedVehicleType))
            throw new InvalidOperationException($"Driver cannot accept {RequestedVehicleType} rides");

        Driver = driver;
        Status = TripStatus.Accepted;
        AcceptedAt = DateTime.UtcNow;
        NotifyObservers("TripAccepted");
    }

    public void MarkDriverEnRoute()
    {
        if (Status != TripStatus.Accepted)
            throw new InvalidTripStateException($"Cannot mark en route from {Status} state");

        Status = TripStatus.DriverEnRoute;
        NotifyObservers("DriverEnRoute");
    }

    public void MarkDriverArrived()
    {
        if (Status != TripStatus.DriverEnRoute)
            throw new InvalidTripStateException($"Cannot mark arrived from {Status} state");

        Status = TripStatus.Arrived;
        NotifyObservers("DriverArrived");
    }

    public void Start()
    {
        if (Status != TripStatus.Arrived)
            throw new InvalidTripStateException($"Cannot start trip from {Status} state");

        Status = TripStatus.PickedUp;
        PickupTime = DateTime.UtcNow;
        Status = TripStatus.InProgress;
        NotifyObservers("TripStarted");
    }

    public void Complete(double distance, Money actualFare)
    {
        if (Status != TripStatus.InProgress)
            throw new InvalidTripStateException($"Cannot complete trip from {Status} state");

        Status = TripStatus.Completed;
        DropoffTime = DateTime.UtcNow;
        Distance = distance;
        ActualFare = actualFare;
        
        if (PickupTime.HasValue)
        {
            Duration = (int)(DropoffTime.Value - PickupTime.Value).TotalMinutes;
        }

        // Update counters
        Rider.IncrementTripCount();
        Driver?.IncrementTripCount();
        Driver?.AddEarnings(actualFare);

        NotifyObservers("TripCompleted");
    }

    public void Cancel(string reason)
    {
        if (Status == TripStatus.Completed || Status == TripStatus.Cancelled)
            throw new InvalidTripStateException($"Cannot cancel trip in {Status} state");

        if (!_currentState?.CanCancel() ?? true)
            throw new InvalidTripStateException($"Cannot cancel trip in {CurrentStateName} state");

        Status = TripStatus.Cancelled;
        CancellationReason = reason;
        NotifyObservers("TripCancelled");
    }

    public void ProcessState()
    {
        _currentState?.Handle(this);
    }

    public void AddObserver(INotificationObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void RemoveObserver(INotificationObserver observer)
    {
        _observers.Remove(observer);
    }

    private void NotifyObservers(string eventType)
    {
        foreach (var observer in _observers)
        {
            observer.OnTripEvent(this, eventType);
        }
    }

    public int GetEstimatedDuration()
    {
        // Rough estimation: 40 km/h average speed in city
        var distance = PickupLocation.DistanceTo(DropoffLocation);
        return (int)Math.Ceiling(distance / 40.0 * 60); // Convert to minutes
    }

    public override string ToString()
    {
        return $"Trip {Id.ToString().Substring(0, 8)} - {Status} - {Rider.Name} with {Driver?.Name ?? "No driver"}";
    }
}
