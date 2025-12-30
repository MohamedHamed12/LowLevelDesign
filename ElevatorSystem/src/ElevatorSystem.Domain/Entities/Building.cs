using ElevatorSystem.Domain.Exceptions;

namespace ElevatorSystem.Domain.Entities;

/// <summary>
/// Represents a building with multiple floors
/// </summary>
public class Building
{
    private readonly Dictionary<int, Floor> _floors;

    /// <summary>
    /// Gets the total number of floors
    /// </summary>
    public int TotalFloors { get; }

    /// <summary>
    /// Gets the minimum floor number (can be negative for basement)
    /// </summary>
    public int MinFloor { get; }

    /// <summary>
    /// Gets the maximum floor number
    /// </summary>
    public int MaxFloor { get; }

    /// <summary>
    /// Gets all floors in the building
    /// </summary>
    public IReadOnlyCollection<Floor> Floors => _floors.Values;

    public Building(int totalFloors, int minFloor = 0)
    {
        if (totalFloors <= 0)
            throw new ArgumentException("Total floors must be positive", nameof(totalFloors));

        TotalFloors = totalFloors;
        MinFloor = minFloor;
        MaxFloor = minFloor + totalFloors - 1;

        _floors = new Dictionary<int, Floor>();
        for (int i = MinFloor; i <= MaxFloor; i++)
        {
            _floors[i] = new Floor(i);
        }
    }

    /// <summary>
    /// Validates if a floor number is valid
    /// </summary>
    public bool IsValidFloor(int floorNumber)
    {
        return floorNumber >= MinFloor && floorNumber <= MaxFloor;
    }

    /// <summary>
    /// Gets a floor by number
    /// </summary>
    public Floor GetFloor(int floorNumber)
    {
        if (!IsValidFloor(floorNumber))
            throw new InvalidFloorException(floorNumber, MinFloor, MaxFloor);

        return _floors[floorNumber];
    }

    /// <summary>
    /// Validates a floor number and throws if invalid
    /// </summary>
    public void ValidateFloor(int floorNumber)
    {
        if (!IsValidFloor(floorNumber))
            throw new InvalidFloorException(floorNumber, MinFloor, MaxFloor);
    }

    public override string ToString()
    {
        return $"Building: {TotalFloors} floors (from {MinFloor} to {MaxFloor})";
    }
}
