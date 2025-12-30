using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;
using RideSharing.Domain.ValueObjects;

namespace RideSharing.Domain.Interfaces;

/// <summary>
/// Defines the contract for driver matching strategies
/// Strategy Pattern implementation
/// </summary>
public interface IDriverMatchingStrategy
{
    /// <summary>
    /// Gets the strategy name
    /// </summary>
    string StrategyName { get; }

    /// <summary>
    /// Finds the most suitable driver for a ride request
    /// </summary>
    /// <param name="availableDrivers">List of available drivers</param>
    /// <param name="pickupLocation">Pickup location</param>
    /// <param name="vehicleType">Requested vehicle type</param>
    /// <returns>Selected driver or null if none available</returns>
    Driver? FindDriver(IEnumerable<Driver> availableDrivers, Location pickupLocation, VehicleType vehicleType);
}
