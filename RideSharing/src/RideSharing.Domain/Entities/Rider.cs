using RideSharing.Domain.Enums;
using RideSharing.Domain.ValueObjects;

namespace RideSharing.Domain.Entities;

/// <summary>
/// Represents a rider (passenger) in the system
/// </summary>
public class Rider : User
{
    public Money WalletBalance { get; private set; }
    public VehicleType PreferredVehicleType { get; private set; }
    public int TotalTrips { get; private set; }
    public Location? CurrentLocation { get; private set; }

    public Rider(string name, string email, string phoneNumber, string passwordHash)
        : base(name, email, phoneNumber, passwordHash, UserType.Rider)
    {
        WalletBalance = Money.Zero();
        PreferredVehicleType = VehicleType.Mini;
        TotalTrips = 0;
    }

    public void AddToWallet(Money amount)
    {
        WalletBalance = WalletBalance.Add(amount);
    }

    public void DeductFromWallet(Money amount)
    {
        if (WalletBalance.IsLessThan(amount))
            throw new InvalidOperationException("Insufficient wallet balance");

        WalletBalance = WalletBalance.Subtract(amount);
    }

    public void SetPreferredVehicleType(VehicleType vehicleType)
    {
        PreferredVehicleType = vehicleType;
    }

    public void UpdateLocation(Location location)
    {
        CurrentLocation = location;
    }

    public void IncrementTripCount()
    {
        TotalTrips++;
    }

    public bool HasSufficientBalance(Money requiredAmount)
    {
        return !WalletBalance.IsLessThan(requiredAmount);
    }
}
