namespace ElevatorSystem.Domain.Enums;

/// <summary>
/// Represents the status of elevator doors
/// </summary>
public enum DoorStatus
{
    /// <summary>
    /// Doors are fully closed
    /// </summary>
    Closed,

    /// <summary>
    /// Doors are fully open
    /// </summary>
    Open,

    /// <summary>
    /// Doors are in the process of opening
    /// </summary>
    Opening,

    /// <summary>
    /// Doors are in the process of closing
    /// </summary>
    Closing
}
