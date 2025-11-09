using ParkingLotSystem.Models.Enums;

namespace ParkingLotSystem.Models.Spots;

public class StandardSpot : ParkingSpot
{
    public StandardSpot(string spotId) : base(spotId, SpotType.Standard)
    {
    }

    public override bool CanFitVehicle(VehicleType vehicleType) 
        => vehicleType is VehicleType.Motorcycle or VehicleType.Car;
}
