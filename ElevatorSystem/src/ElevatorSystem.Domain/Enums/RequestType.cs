namespace ElevatorSystem.Domain.Enums;

/// <summary>
/// Represents the type of elevator request
/// </summary>
public enum RequestType
{
    /// <summary>
    /// Request made from outside the elevator (hall call)
    /// </summary>
    External,

    /// <summary>
    /// Request made from inside the elevator (car call)
    /// </summary>
    Internal
}
