using RideSharing.Domain.Enums;
using RideSharing.Domain.ValueObjects;

namespace RideSharing.Domain.Entities;

/// <summary>
/// Represents a driver in the system
/// </summary>
public class Driver : User
{
    private readonly object _locationLock = new();

    public string LicenseNumber { get; private set; }
    public Vehicle? Vehicle { get; private set; }
    public bool IsAvailable { get; private set; }
    public Location? CurrentLocation { get; private set; }
    public int TotalTrips { get; private set; }
    public Money TotalEarnings { get; private set; }
    public bool IsDocumentVerified { get; private set; }
    public DateTime? LastLocationUpdate { get; private set; }

    public Driver(string name, string email, string phoneNumber, string passwordHash, string licenseNumber)
        : base(name, email, phoneNumber, passwordHash, UserType.Driver)
    {
        LicenseNumber = licenseNumber ?? throw new ArgumentNullException(nameof(licenseNumber));
        IsAvailable = false;
        TotalTrips = 0;
        TotalEarnings = Money.Zero();
        IsDocumentVerified = false;
    }

    public void AssignVehicle(Vehicle vehicle)
    {
        Vehicle = vehicle ?? throw new ArgumentNullException(nameof(vehicle));
    }

    public void GoOnline()
    {
        if (Vehicle == null)
            throw new InvalidOperationException("Cannot go online without an assigned vehicle");

        if (!IsDocumentVerified)
            throw new InvalidOperationException("Cannot go online without verified documents");

        IsAvailable = true;
    }

    public void GoOffline()
    {
        IsAvailable = false;
    }

    public void UpdateLocation(Location location)
    {
        lock (_locationLock)
        {
            CurrentLocation = location;
            LastLocationUpdate = DateTime.UtcNow;
        }
    }

    public void VerifyDocuments()
    {
        IsDocumentVerified = true;
    }

    public void IncrementTripCount()
    {
        TotalTrips++;
    }

    public void AddEarnings(Money amount)
    {
        TotalEarnings = TotalEarnings.Add(amount);
    }

    public double? DistanceFrom(Location location)
    {
        return CurrentLocation?.DistanceTo(location);
    }

    public bool IsNearby(Location location, double radiusKm)
    {
        if (CurrentLocation == null) return false;
        return CurrentLocation.IsNearby(location, radiusKm);
    }

    public bool CanAcceptVehicleType(VehicleType requestedType)
    {
        if (Vehicle == null) return false;
        return Vehicle.VehicleType == requestedType;
    }
}
