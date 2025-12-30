using ElevatorSystem.Domain.Entities;

namespace ElevatorSystem.Domain.Interfaces;

/// <summary>
/// Defines the contract for elevator state behavior
/// State Pattern implementation
/// </summary>
public interface IElevatorState
{
    /// <summary>
    /// Handles the elevator behavior in current state
    /// </summary>
    void Handle(Elevator elevator);

    /// <summary>
    /// Determines if the elevator can accept new requests in this state
    /// </summary>
    bool CanAcceptRequest();

    /// <summary>
    /// Gets the state name
    /// </summary>
    string StateName { get; }
}
