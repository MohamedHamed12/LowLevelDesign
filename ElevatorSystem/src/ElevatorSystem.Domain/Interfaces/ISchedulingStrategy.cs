using ElevatorSystem.Domain.Entities;

namespace ElevatorSystem.Domain.Interfaces;

/// <summary>
/// Defines the contract for elevator scheduling strategies
/// Strategy Pattern implementation
/// </summary>
public interface ISchedulingStrategy
{
    /// <summary>
    /// Selects the most appropriate elevator for a given request
    /// </summary>
    /// <param name="elevators">Available elevators</param>
    /// <param name="request">The elevator request</param>
    /// <returns>Selected elevator or null if none available</returns>
    Elevator? SelectElevator(IEnumerable<Elevator> elevators, Request request);

    /// <summary>
    /// Gets the strategy name
    /// </summary>
    string StrategyName { get; }
}
