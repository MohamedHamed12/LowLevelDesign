using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Enums;
using ElevatorSystem.Domain.Interfaces;

namespace ElevatorSystem.Application.Strategies;

/// <summary>
/// Selects the nearest available elevator
/// </summary>
public class NearestElevatorStrategy : ISchedulingStrategy
{
    public string StrategyName => "Nearest Elevator";

    public Elevator? SelectElevator(IEnumerable<Elevator> elevators, Request request)
    {
        var availableElevators = elevators.Where(e => e.IsAvailable).ToList();
        
        if (!availableElevators.Any())
            return null;

        // Find elevator with minimum distance to request floor
        Elevator? nearestElevator = null;
        int minDistance = int.MaxValue;

        foreach (var elevator in availableElevators)
        {
            int distance = elevator.DistanceToFloor(request.SourceFloor);
            
            // Prefer elevators moving in the same direction
            bool sameDirection = (request.Direction == Direction.Up && elevator.Direction == Direction.Up) ||
                                (request.Direction == Direction.Down && elevator.Direction == Direction.Down);
            
            // Adjust distance based on direction compatibility
            int adjustedDistance = sameDirection ? distance : distance + 100;

            if (adjustedDistance < minDistance)
            {
                minDistance = adjustedDistance;
                nearestElevator = elevator;
            }
        }

        return nearestElevator;
    }
}
