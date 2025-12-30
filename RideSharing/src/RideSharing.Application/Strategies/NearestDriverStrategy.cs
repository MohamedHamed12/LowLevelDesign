using RideSharing.Domain.Entities;
using RideSharing.Domain.Interfaces;
using RideSharing.Domain.ValueObjects;
using RideSharing.Domain.Enums;

namespace RideSharing.Application.Strategies;

/// <summary>
/// Finds the nearest available driver
/// </summary>
public class NearestDriverStrategy : IDriverMatchingStrategy
{
    public string StrategyName => "Nearest Driver";

    public Driver? FindDriver(IEnumerable<Driver> availableDrivers, Location pickupLocation, VehicleType vehicleType)
    {
        return availableDrivers
            .Where(d => d.IsAvailable && 
                       d.IsNearby(pickupLocation, radiusKm: 10) &&
                       d.Vehicle != null &&
                       d.Vehicle.VehicleType == vehicleType)
            .OrderBy(d => d.CurrentLocation!.DistanceTo(pickupLocation))
            .FirstOrDefault();
    }
}
