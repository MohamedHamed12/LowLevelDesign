using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;
using RideSharing.Domain.ValueObjects;

namespace RideSharing.Domain.Interfaces;

/// <summary>
/// Defines the contract for pricing strategies
/// Strategy Pattern implementation
/// </summary>
public interface IPricingStrategy
{
    /// <summary>
    /// Gets the strategy name
    /// </summary>
    string StrategyName { get; }

    /// <summary>
    /// Calculates the fare for a trip
    /// </summary>
    /// <param name="distance">Trip distance in kilometers</param>
    /// <param name="duration">Trip duration in minutes</param>
    /// <param name="vehicleType">Vehicle type</param>
    /// <returns>Calculated fare</returns>
    Money CalculateFare(double distance, int duration, VehicleType vehicleType);

    /// <summary>
    /// Estimates fare before trip starts
    /// </summary>
    Money EstimateFare(double estimatedDistance, int estimatedDuration, VehicleType vehicleType);
}
