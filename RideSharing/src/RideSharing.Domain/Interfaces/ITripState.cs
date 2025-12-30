using RideSharing.Domain.Entities;

namespace RideSharing.Domain.Interfaces;

/// <summary>
/// Defines the contract for trip state behavior
/// State Pattern implementation
/// </summary>
public interface ITripState
{
    /// <summary>
    /// Gets the state name
    /// </summary>
    string StateName { get; }

    /// <summary>
    /// Handles the trip in current state
    /// </summary>
    void Handle(Trip trip);

    /// <summary>
    /// Determines if the trip can be cancelled in this state
    /// </summary>
    bool CanCancel();

    /// <summary>
    /// Determines if the trip can transition to another state
    /// </summary>
    bool CanTransitionTo(string newState);
}
