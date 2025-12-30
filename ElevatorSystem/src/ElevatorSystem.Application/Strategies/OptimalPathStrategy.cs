using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Enums;
using ElevatorSystem.Domain.Interfaces;

namespace ElevatorSystem.Application.Strategies;

/// <summary>
/// Selects elevator based on optimal path considering direction and distance
/// </summary>
public class OptimalPathStrategy : ISchedulingStrategy
{
    public string StrategyName => "Optimal Path";

    public Elevator? SelectElevator(IEnumerable<Elevator> elevators, Request request)
    {
        var availableElevators = elevators.Where(e => e.IsAvailable).ToList();
        
        if (!availableElevators.Any())
            return null;

        Elevator? bestElevator = null;
        double bestScore = double.MaxValue;

        foreach (var elevator in availableElevators)
        {
            double score = CalculateScore(elevator, request);
            
            if (score < bestScore)
            {
                bestScore = score;
                bestElevator = elevator;
            }
        }

        return bestElevator;
    }

    private double CalculateScore(Elevator elevator, Request request)
    {
        double score = 0;

        // Distance factor
        int distance = elevator.DistanceToFloor(request.SourceFloor);
        score += distance * 10;

        // Direction compatibility factor
        bool movingTowardRequest = 
            (elevator.Direction == Direction.Up && request.SourceFloor > elevator.CurrentFloor) ||
            (elevator.Direction == Direction.Down && request.SourceFloor < elevator.CurrentFloor);

        bool sameDirection = elevator.Direction == request.Direction;

        if (elevator.Direction == Direction.None)
        {
            // Idle elevator - best case
            score += 0;
        }
        else if (movingTowardRequest && sameDirection)
        {
            // Moving toward request in same direction - good
            score += 5;
        }
        else if (movingTowardRequest)
        {
            // Moving toward but opposite direction - okay
            score += 20;
        }
        else
        {
            // Moving away - not ideal
            score += 50;
        }

        // Load factor
        score += elevator.GetPendingRequestCount() * 3;

        return score;
    }
}
