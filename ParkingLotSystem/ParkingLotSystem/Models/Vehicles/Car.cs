using ParkingLotSystem.Models.Enums;

namespace ParkingLotSystem.Models.Vehicles;

public class Car : Vehicle
{
    public Car(string licensePlate) 
        : base(licensePlate, VehicleType.Car)
    {
    }

    public override SpotType GetRequiredSpotType() => SpotType.Standard;
}
