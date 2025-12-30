namespace ElevatorSystem.Domain.Enums;

/// <summary>
/// Represents the current operational state of an elevator
/// </summary>
public enum ElevatorState
{
    /// <summary>
    /// Elevator is stationary and waiting for requests
    /// </summary>
    Idle,

    /// <summary>
    /// Elevator is moving upward
    /// </summary>
    MovingUp,

    /// <summary>
    /// Elevator is moving downward
    /// </summary>
    MovingDown,

    /// <summary>
    /// Elevator is out of service for maintenance
    /// </summary>
    Maintenance
}
