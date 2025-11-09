using ParkingLotSystem.Models.Enums;

namespace ParkingLotSystem.Models.Spots;

public class ElectricSpot : ParkingSpot
{
    public ElectricSpot(string spotId) : base(spotId, SpotType.Electric)
    {
    }

    public override bool CanFitVehicle(VehicleType vehicleType) 
        => vehicleType == VehicleType.ElectricCar;
}
