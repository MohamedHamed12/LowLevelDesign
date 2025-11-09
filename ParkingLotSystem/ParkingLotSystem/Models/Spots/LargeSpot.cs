using ParkingLotSystem.Models.Enums;

namespace ParkingLotSystem.Models.Spots;

public class LargeSpot : ParkingSpot
{
    public LargeSpot(string spotId) : base(spotId, SpotType.Large)
    {
    }

    public override bool CanFitVehicle(VehicleType vehicleType) => true; // Can fit all vehicles
}
