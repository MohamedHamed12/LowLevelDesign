using ElevatorSystem.Domain.Entities;

namespace ElevatorSystem.Domain.Interfaces;

/// <summary>
/// Defines the contract for elevator factory
/// Factory Pattern implementation
/// </summary>
public interface IElevatorFactory
{
    /// <summary>
    /// Creates a new elevator instance
    /// </summary>
    /// <param name="id">Elevator identifier</param>
    /// <param name="totalFloors">Total floors in the building</param>
    /// <returns>New elevator instance</returns>
    Elevator CreateElevator(int id, int totalFloors);

    /// <summary>
    /// Creates a new elevator with custom capacity
    /// </summary>
    /// <param name="id">Elevator identifier</param>
    /// <param name="totalFloors">Total floors in the building</param>
    /// <param name="capacity">Maximum capacity</param>
    /// <returns>New elevator instance</returns>
    Elevator CreateElevator(int id, int totalFloors, int capacity);
}
