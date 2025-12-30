using ElevatorSystem.Application.Strategies;
using ElevatorSystem.Application.States;
using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Enums;
using ElevatorSystem.Domain.Interfaces;
using Xunit;

namespace ElevatorSystem.Tests.Application;

public class SchedulingStrategyTests
{
    [Fact]
    public void NearestElevatorStrategy_ShouldSelectClosestElevator()
    {
        // Arrange
        var elevator1 = new Elevator(1, 10);
        elevator1.SetState(new IdleState());
        // Elevator 1 is at floor 0

        var elevator2 = new Elevator(2, 10);
        elevator2.SetState(new IdleState());
        elevator2.MoveUp();
        elevator2.MoveUp();
        elevator2.MoveUp();
        elevator2.MoveUp();
        elevator2.MoveUp();
        // Elevator 2 is at floor 5

        var elevators = new List<Elevator> { elevator1, elevator2 };
        var request = new Request(4, Direction.Up, RequestType.External);
        var strategy = new NearestElevatorStrategy();

        // Act
        var selected = strategy.SelectElevator(elevators, request);

        // Assert
        Assert.NotNull(selected);
        Assert.Equal(2, selected.Id); // Elevator 2 is closer to floor 4
    }

    [Fact]
    public void LeastLoadedStrategy_ShouldSelectElevatorWithFewestRequests()
    {
        // Arrange
        var elevator1 = new Elevator(1, 10);
        elevator1.SetState(new IdleState());
        elevator1.AddRequest(3);
        elevator1.AddRequest(5);
        elevator1.AddRequest(7);

        var elevator2 = new Elevator(2, 10);
        elevator2.SetState(new IdleState());
        elevator2.AddRequest(4);

        var elevators = new List<Elevator> { elevator1, elevator2 };
        var request = new Request(6, Direction.Up, RequestType.External);
        var strategy = new LeastLoadedStrategy();

        // Act
        var selected = strategy.SelectElevator(elevators, request);

        // Assert
        Assert.NotNull(selected);
        Assert.Equal(2, selected.Id); // Elevator 2 has fewer requests
    }

    [Fact]
    public void OptimalPathStrategy_ShouldSelectBestElevator()
    {
        // Arrange
        var elevator1 = new Elevator(1, 10);
        elevator1.SetState(new IdleState());

        var elevator2 = new Elevator(2, 10);
        elevator2.SetState(new IdleState());

        var elevators = new List<Elevator> { elevator1, elevator2 };
        var request = new Request(5, Direction.Up, RequestType.External);
        var strategy = new OptimalPathStrategy();

        // Act
        var selected = strategy.SelectElevator(elevators, request);

        // Assert
        Assert.NotNull(selected);
    }

    [Fact]
    public void AllStrategies_ShouldReturnNullWhenNoElevatorsAvailable()
    {
        // Arrange
        var elevators = new List<Elevator>();
        var request = new Request(5, Direction.Up, RequestType.External);

        var strategies = new List<ISchedulingStrategy>
        {
            new NearestElevatorStrategy(),
            new LeastLoadedStrategy(),
            new OptimalPathStrategy()
        };

        // Act & Assert
        foreach (var strategy in strategies)
        {
            var selected = strategy.SelectElevator(elevators, request);
            Assert.Null(selected);
        }
    }
}
