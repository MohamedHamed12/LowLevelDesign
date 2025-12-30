namespace ElevatorSystem.Domain.Exceptions;

/// <summary>
/// Exception thrown when an invalid floor number is provided
/// </summary>
public class InvalidFloorException : Exception
{
    public int InvalidFloor { get; }
    public int MinFloor { get; }
    public int MaxFloor { get; }

    public InvalidFloorException(int invalidFloor, int minFloor, int maxFloor)
        : base($"Floor {invalidFloor} is invalid. Valid range is {minFloor} to {maxFloor}.")
    {
        InvalidFloor = invalidFloor;
        MinFloor = minFloor;
        MaxFloor = maxFloor;
    }
}
