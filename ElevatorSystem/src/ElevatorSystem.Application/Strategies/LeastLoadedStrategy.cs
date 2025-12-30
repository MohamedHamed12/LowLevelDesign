using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Interfaces;

namespace ElevatorSystem.Application.Strategies;

/// <summary>
/// Selects the elevator with the least number of pending requests
/// </summary>
public class LeastLoadedStrategy : ISchedulingStrategy
{
    public string StrategyName => "Least Loaded";

    public Elevator? SelectElevator(IEnumerable<Elevator> elevators, Request request)
    {
        var availableElevators = elevators.Where(e => e.IsAvailable).ToList();
        
        if (!availableElevators.Any())
            return null;

        // Find elevator with minimum pending requests
        return availableElevators
            .OrderBy(e => e.GetPendingRequestCount())
            .ThenBy(e => e.DistanceToFloor(request.SourceFloor))
            .FirstOrDefault();
    }
}
