using ElevatorSystem.Domain.Entities;

namespace ElevatorSystem.Domain.Interfaces;

/// <summary>
/// Defines the contract for elevator observers
/// Observer Pattern implementation
/// </summary>
public interface IElevatorObserver
{
    /// <summary>
    /// Called when the elevator state changes
    /// </summary>
    /// <param name="elevator">The elevator that changed</param>
    void Update(Elevator elevator);

    /// <summary>
    /// Gets the observer name
    /// </summary>
    string ObserverName { get; }
}
