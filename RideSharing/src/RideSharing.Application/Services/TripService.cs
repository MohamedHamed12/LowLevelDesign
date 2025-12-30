using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;
using RideSharing.Domain.Interfaces;
using RideSharing.Domain.ValueObjects;
using RideSharing.Domain.Exceptions;
using RideSharing.Application.States;

namespace RideSharing.Application.Services;

/// <summary>
/// Service for managing trip lifecycle
/// </summary>
public class TripService
{
    private readonly IDriverMatchingStrategy _driverMatchingStrategy;
    private readonly IPricingStrategy _pricingStrategy;
    private readonly List<Driver> _drivers; // In real app, this would be a repository
    private readonly Dictionary<TripStatus, ITripState> _states;

    public TripService(
        IDriverMatchingStrategy driverMatchingStrategy,
        IPricingStrategy pricingStrategy,
        List<Driver> drivers)
    {
        _driverMatchingStrategy = driverMatchingStrategy;
        _pricingStrategy = pricingStrategy;
        _drivers = drivers;

        // Initialize states
        _states = new Dictionary<TripStatus, ITripState>
        {
            { TripStatus.Requested, new RequestedState() },
            { TripStatus.Accepted, new AcceptedState() },
            { TripStatus.InProgress, new InProgressState() },
            { TripStatus.Completed, new CompletedState() },
            { TripStatus.Cancelled, new CancelledState() }
        };
    }

    public Trip RequestTrip(Rider rider, Location pickup, Location dropoff, VehicleType vehicleType)
    {
        var distance = pickup.DistanceTo(dropoff);
        var estimatedDuration = EstimateDuration(pickup, dropoff);
        var estimatedFare = _pricingStrategy.EstimateFare(distance, (int)estimatedDuration.TotalMinutes, vehicleType);

        var trip = new Trip(rider, pickup, dropoff, vehicleType);
        trip.SetEstimatedFare(estimatedFare);
        trip.SetState(_states[TripStatus.Requested]);

        return trip;
    }

    public bool AssignDriver(Trip trip)
    {
        if (trip.Status != TripStatus.Requested)
            throw new InvalidTripStateException("Can only assign driver to requested trips");

        var driver = _driverMatchingStrategy.FindDriver(_drivers, trip.PickupLocation, trip.RequestedVehicleType);
        
        if (driver == null)
            throw new DriverNotAvailableException("No available drivers found in your area");

        trip.AssignDriver(driver);
        trip.SetState(_states[TripStatus.Accepted]);
        
        return true;
    }

    public void StartTrip(Trip trip)
    {
        if (trip.Status != TripStatus.Accepted)
            throw new InvalidTripStateException("Trip must be accepted before starting");

        trip.Start();
        trip.SetState(_states[TripStatus.InProgress]);
    }

    public void CompleteTrip(Trip trip, Location finalLocation)
    {
        if (trip.Status != TripStatus.InProgress)
            throw new InvalidTripStateException("Trip must be in progress to complete");

        var actualDistance = trip.PickupLocation.DistanceTo(finalLocation);
        var actualDuration = DateTime.UtcNow - (trip.PickupTime ?? DateTime.UtcNow);
        
        var actualFare = _pricingStrategy.CalculateFare(
            actualDistance,
            (int)actualDuration.TotalMinutes,
            trip.RequestedVehicleType);

        trip.Complete(actualDistance, actualFare);
        trip.SetState(_states[TripStatus.Completed]);
    }

    public void CancelTrip(Trip trip, string reason)
    {
        if (trip.Status == TripStatus.Completed)
            throw new InvalidTripStateException("Cannot cancel completed trip");

        if (trip.Status == TripStatus.InProgress)
            throw new InvalidTripStateException("Cannot cancel trip in progress");

        trip.Cancel(reason);
        trip.SetState(_states[TripStatus.Cancelled]);
    }

    public Money GetFareEstimate(Location pickup, Location dropoff, VehicleType vehicleType)
    {
        var distance = pickup.DistanceTo(dropoff);
        var estimatedDuration = EstimateDuration(pickup, dropoff);
        return _pricingStrategy.EstimateFare(distance, (int)estimatedDuration.TotalMinutes, vehicleType);
    }

    private TimeSpan EstimateDuration(Location pickup, Location dropoff)
    {
        var distance = pickup.DistanceTo(dropoff);
        // Assume average speed of 40 km/h in city
        var hours = distance / 40.0;
        return TimeSpan.FromHours(hours);
    }
}
