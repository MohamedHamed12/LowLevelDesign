using ElevatorSystem.Application.States;
using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Interfaces;

namespace ElevatorSystem.Application.Factories;

/// <summary>
/// Default factory for creating elevator instances
/// </summary>
public class DefaultElevatorFactory : IElevatorFactory
{
    public Elevator CreateElevator(int id, int totalFloors)
    {
        return CreateElevator(id, totalFloors, capacity: 10);
    }

    public Elevator CreateElevator(int id, int totalFloors, int capacity)
    {
        var elevator = new Elevator(id, totalFloors, capacity);
        
        // Set initial state to Idle
        elevator.SetState(new IdleState());
        
        return elevator;
    }
}
