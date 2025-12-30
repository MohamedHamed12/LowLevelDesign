using RideSharing.Domain.Entities;
using RideSharing.Domain.Interfaces;
using RideSharing.Domain.ValueObjects;
using RideSharing.Domain.Enums;

namespace RideSharing.Application.Strategies;

/// <summary>
/// Standard pricing based on distance and time
/// </summary>
public class StandardPricingStrategy : IPricingStrategy
{
    public string StrategyName => "Standard Pricing";

    public Money CalculateFare(double distance, int duration, VehicleType vehicleType)
    {
        var fare = Fare.Calculate(vehicleType, distance, TimeSpan.FromMinutes(duration), surgeMultiplier: 1.0m);
        return fare.TotalFare;
    }

    public Money EstimateFare(double estimatedDistance, int estimatedDuration, VehicleType vehicleType)
    {
        return CalculateFare(estimatedDistance, estimatedDuration, vehicleType);
    }
}
