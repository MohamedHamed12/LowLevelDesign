using ParkingLotSystem.Models.Enums;

namespace ParkingLotSystem.Models.Spots;

public class CompactSpot : ParkingSpot
{
    public CompactSpot(string spotId) : base(spotId, SpotType.Compact)
    {
    }

    public override bool CanFitVehicle(VehicleType vehicleType) 
        => vehicleType == VehicleType.Motorcycle;
}
