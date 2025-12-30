using RideSharing.Domain.Entities;
using RideSharing.Domain.Interfaces;
using RideSharing.Domain.ValueObjects;
using RideSharing.Domain.Enums;

namespace RideSharing.Application.Strategies;

/// <summary>
/// Finds drivers with highest ratings first
/// </summary>
public class HighestRatedDriverStrategy : IDriverMatchingStrategy
{
    public string StrategyName => "Highest Rated Driver";

    public Driver? FindDriver(IEnumerable<Driver> availableDrivers, Location pickupLocation, VehicleType vehicleType)
    {
        return availableDrivers
            .Where(d => d.IsAvailable && 
                       d.IsNearby(pickupLocation, radiusKm: 10) &&
                       d.Vehicle != null &&
                       d.Vehicle.VehicleType == vehicleType)
            .OrderByDescending(d => d.Rating.Value)
            .ThenBy(d => d.CurrentLocation!.DistanceTo(pickupLocation))
            .FirstOrDefault();
    }
}
