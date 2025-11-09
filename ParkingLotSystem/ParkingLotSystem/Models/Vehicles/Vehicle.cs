using ParkingLotSystem.Models.Enums;

namespace ParkingLotSystem.Models.Vehicles;

public abstract class Vehicle
{
    public string LicensePlate { get; }
    public VehicleType Type { get; }

    protected Vehicle(string licensePlate, VehicleType type)
    {
        LicensePlate = licensePlate ?? throw new ArgumentNullException(nameof(licensePlate));
        Type = type;
    }

    public abstract SpotType GetRequiredSpotType();

    public override string ToString() => $"{Type} ({LicensePlate})";
}
