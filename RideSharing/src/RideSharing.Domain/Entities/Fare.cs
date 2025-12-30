using RideSharing.Domain.Enums;
using RideSharing.Domain.ValueObjects;

namespace RideSharing.Domain.Entities;

/// <summary>
/// Represents the fare breakdown for a trip
/// </summary>
public class Fare
{
    public Guid Id { get; private set; }
    public Money BaseFare { get; private set; }
    public Money DistanceFare { get; private set; }
    public Money TimeFare { get; private set; }
    public Money SurgeMultiplier { get; private set; }
    public Money Discount { get; private set; }
    public Money TaxAmount { get; private set; }
    public Money TotalFare { get; private set; }
    public VehicleType VehicleType { get; private set; }
    public double DistanceInKm { get; private set; }
    public TimeSpan Duration { get; private set; }
    public DateTime CalculatedAt { get; private set; }

    // Suppress warnings - this is for ORM only
#pragma warning disable CS8618
    private Fare() { }
#pragma warning restore CS8618

    public Fare(
        VehicleType vehicleType,
        double distanceInKm,
        TimeSpan duration,
        Money baseFare,
        Money distanceFare,
        Money timeFare,
        decimal surgeMultiplier = 1.0m,
        Money? discount = null)
    {
        Id = Guid.NewGuid();
        VehicleType = vehicleType;
        DistanceInKm = distanceInKm;
        Duration = duration;
        BaseFare = baseFare;
        DistanceFare = distanceFare;
        TimeFare = timeFare;
        SurgeMultiplier = new Money(surgeMultiplier, baseFare.Currency);
        Discount = discount ?? Money.Zero(baseFare.Currency);
        CalculatedAt = DateTime.UtcNow;

        // Calculate total before tax
        var subtotal = (BaseFare + DistanceFare + TimeFare) * surgeMultiplier;
        subtotal = subtotal - Discount;

        // Calculate tax (assuming 10% tax)
        TaxAmount = subtotal * 0.10m;

        // Calculate total
        TotalFare = subtotal + TaxAmount;
    }

    public static Fare Calculate(
        VehicleType vehicleType,
        double distanceInKm,
        TimeSpan duration,
        decimal surgeMultiplier = 1.0m,
        Money? discount = null)
    {
        // Base fare varies by vehicle type
        var baseFare = vehicleType switch
        {
            VehicleType.Bike => new Money(30m, "USD"),
            VehicleType.Auto => new Money(50m, "USD"),
            VehicleType.Mini => new Money(70m, "USD"),
            VehicleType.Sedan => new Money(100m, "USD"),
            VehicleType.SUV => new Money(150m, "USD"),
            VehicleType.Luxury => new Money(250m, "USD"),
            _ => new Money(70m, "USD")
        };

        // Distance rate varies by vehicle type (per km)
        var distanceRate = vehicleType switch
        {
            VehicleType.Bike => 8m,
            VehicleType.Auto => 12m,
            VehicleType.Mini => 15m,
            VehicleType.Sedan => 20m,
            VehicleType.SUV => 25m,
            VehicleType.Luxury => 40m,
            _ => 15m
        };

        // Time rate (per minute)
        var timeRate = vehicleType switch
        {
            VehicleType.Bike => 1m,
            VehicleType.Auto => 1.5m,
            VehicleType.Mini => 2m,
            VehicleType.Sedan => 2.5m,
            VehicleType.SUV => 3m,
            VehicleType.Luxury => 5m,
            _ => 2m
        };

        var distanceFare = new Money((decimal)distanceInKm * distanceRate, "USD");
        var timeFare = new Money((decimal)duration.TotalMinutes * timeRate, "USD");

        return new Fare(
            vehicleType,
            distanceInKm,
            duration,
            baseFare,
            distanceFare,
            timeFare,
            surgeMultiplier,
            discount);
    }

    public string GetBreakdown()
    {
        var breakdown = $"""
            Fare Breakdown:
            ---------------
            Base Fare:      {BaseFare}
            Distance Fare:  {DistanceFare} ({DistanceInKm:F2} km)
            Time Fare:      {TimeFare} ({Duration.TotalMinutes:F0} mins)
            Subtotal:       {BaseFare + DistanceFare + TimeFare}
            Surge (x{SurgeMultiplier.Amount:F2}):  {(BaseFare + DistanceFare + TimeFare) * (SurgeMultiplier.Amount - 1)}
            Discount:       -{Discount}
            Tax (10%):      {TaxAmount}
            ---------------
            Total:          {TotalFare}
            """;

        return breakdown;
    }

    public override string ToString()
    {
        return $"Fare: {TotalFare} ({VehicleType})";
    }
}
