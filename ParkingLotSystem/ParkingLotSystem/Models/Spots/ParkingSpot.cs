using ParkingLotSystem.Models.Enums;
using ParkingLotSystem.Models.Vehicles;

namespace ParkingLotSystem.Models.Spots;

public abstract class ParkingSpot
{
    private readonly object _lock = new();

    public string SpotId { get; }
    public SpotType Type { get; }
    public bool IsOccupied { get; private set; }
    public Vehicle? CurrentVehicle { get; private set; }

    protected ParkingSpot(string spotId, SpotType type)
    {
        SpotId = spotId ?? throw new ArgumentNullException(nameof(spotId));
        Type = type;
        IsOccupied = false;
    }

    public bool IsAvailable() => !IsOccupied;

    public abstract bool CanFitVehicle(VehicleType vehicleType);

    public bool OccupySpot(Vehicle vehicle)
    {
        lock (_lock)
        {
            if (IsOccupied || !CanFitVehicle(vehicle.Type))
                return false;

            CurrentVehicle = vehicle;
            IsOccupied = true;
            return true;
        }
    }

    public void ReleaseSpot()
    {
        lock (_lock)
        {
            CurrentVehicle = null;
            IsOccupied = false;
        }
    }

    public override string ToString() => $"{Type} Spot {SpotId} - {(IsOccupied ? "Occupied" : "Available")}";
}
