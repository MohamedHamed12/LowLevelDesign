using RideSharing.Domain.Entities;
using RideSharing.Domain.Interfaces;
using RideSharing.Domain.ValueObjects;
using RideSharing.Domain.Enums;

namespace RideSharing.Application.Strategies;

/// <summary>
/// Surge pricing based on demand and supply
/// </summary>
public class SurgePricingStrategy : IPricingStrategy
{
    private readonly double _baseSurgeMultiplier;

    public string StrategyName => "Surge Pricing";

    public SurgePricingStrategy(double surgeMultiplier = 1.5)
    {
        _baseSurgeMultiplier = surgeMultiplier;
    }

    public Money CalculateFare(double distance, int duration, VehicleType vehicleType)
    {
        var surgeFactor = CalculateDynamicSurge(DateTime.UtcNow);
        var fare = Fare.Calculate(vehicleType, distance, TimeSpan.FromMinutes(duration), surgeMultiplier: (decimal)surgeFactor);
        return fare.TotalFare;
    }

    public Money EstimateFare(double estimatedDistance, int estimatedDuration, VehicleType vehicleType)
    {
        return CalculateFare(estimatedDistance, estimatedDuration, vehicleType);
    }

    private double CalculateDynamicSurge(DateTime currentTime)
    {
        var hour = currentTime.Hour;
        
        // Peak hours (7-10 AM, 5-8 PM): higher surge
        if ((hour >= 7 && hour <= 10) || (hour >= 17 && hour <= 20))
        {
            return _baseSurgeMultiplier * 1.5; // 2.25x
        }
        
        // Late night (10 PM - 4 AM): moderate surge
        if (hour >= 22 || hour <= 4)
        {
            return _baseSurgeMultiplier * 1.2; // 1.8x
        }
        
        // Normal hours
        return _baseSurgeMultiplier;
    }
}
