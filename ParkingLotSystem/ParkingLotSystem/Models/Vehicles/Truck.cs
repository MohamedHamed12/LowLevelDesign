using ParkingLotSystem.Models.Enums;

namespace ParkingLotSystem.Models.Vehicles;

public class Truck : Vehicle
{
    public Truck(string licensePlate) 
        : base(licensePlate, VehicleType.Truck)
    {
    }

    public override SpotType GetRequiredSpotType() => SpotType.Large;
}
