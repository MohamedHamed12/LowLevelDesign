using ParkingLotSystem.Models.Enums;

namespace ParkingLotSystem.Models.Vehicles;

public class ElectricCar : Vehicle
{
    public ElectricCar(string licensePlate) 
        : base(licensePlate, VehicleType.ElectricCar)
    {
    }

    public override SpotType GetRequiredSpotType() => SpotType.Electric;
}
