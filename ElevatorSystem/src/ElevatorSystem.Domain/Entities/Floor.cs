using ElevatorSystem.Domain.Enums;
using ElevatorSystem.Domain.Exceptions;

namespace ElevatorSystem.Domain.Entities;

/// <summary>
/// Represents a building floor
/// </summary>
public class Floor
{
    private readonly object _lock = new();

    /// <summary>
    /// Gets the floor number
    /// </summary>
    public int FloorNumber { get; }

    /// <summary>
    /// Gets or sets whether there's an active up request
    /// </summary>
    public bool HasUpRequest { get; private set; }

    /// <summary>
    /// Gets or sets whether there's an active down request
    /// </summary>
    public bool HasDownRequest { get; private set; }

    public Floor(int floorNumber)
    {
        FloorNumber = floorNumber;
    }

    /// <summary>
    /// Registers a request for this floor
    /// </summary>
    public void RegisterRequest(Direction direction)
    {
        lock (_lock)
        {
            switch (direction)
            {
                case Direction.Up:
                    HasUpRequest = true;
                    break;
                case Direction.Down:
                    HasDownRequest = true;
                    break;
                case Direction.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }
        }
    }

    /// <summary>
    /// Clears the request for this floor
    /// </summary>
    public void ClearRequest(Direction direction)
    {
        lock (_lock)
        {
            switch (direction)
            {
                case Direction.Up:
                    HasUpRequest = false;
                    break;
                case Direction.Down:
                    HasDownRequest = false;
                    break;
                case Direction.None:
                    HasUpRequest = false;
                    HasDownRequest = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }
        }
    }

    /// <summary>
    /// Checks if there's any pending request
    /// </summary>
    public bool HasPendingRequest()
    {
        lock (_lock)
        {
            return HasUpRequest || HasDownRequest;
        }
    }

    public override string ToString()
    {
        return $"Floor {FloorNumber} (Up: {HasUpRequest}, Down: {HasDownRequest})";
    }
}
