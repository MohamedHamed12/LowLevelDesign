using ParkingLotSystem.Models.Enums;

namespace ParkingLotSystem.Models.Vehicles;

public class Motorcycle : Vehicle
{
    public Motorcycle(string licensePlate) 
        : base(licensePlate, VehicleType.Motorcycle)
    {
    }

    public override SpotType GetRequiredSpotType() => SpotType.Compact;
}
