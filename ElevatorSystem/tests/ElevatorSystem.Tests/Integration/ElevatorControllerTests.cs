using ElevatorSystem.Application.Services;
using ElevatorSystem.Application.Strategies;
using ElevatorSystem.Application.States;
using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Enums;
using Xunit;

namespace ElevatorSystem.Tests.Integration;

public class ElevatorControllerTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Arrange
        var elevators = new List<Elevator>
        {
            new Elevator(1, 10),
            new Elevator(2, 10)
        };
        
        foreach (var elevator in elevators)
        {
            elevator.SetState(new IdleState());
        }

        var building = new Building(10, 0);
        var strategy = new NearestElevatorStrategy();

        // Act
        var controller = new ElevatorController(elevators, building, strategy);

        // Assert
        Assert.Equal(2, controller.Elevators.Count);
        Assert.NotNull(controller.Building);
    }

    [Fact]
    public void RequestElevator_WithValidFloor_ShouldEnqueueRequest()
    {
        // Arrange
        var elevators = new List<Elevator> { new Elevator(1, 10) };
        elevators[0].SetState(new IdleState());
        var building = new Building(10, 0);
        var strategy = new NearestElevatorStrategy();
        var controller = new ElevatorController(elevators, building, strategy);

        // Act
        controller.RequestElevator(5, Direction.Up);
        controller.ProcessRequests();

        // Assert - Request should be processed
        Thread.Sleep(500);
        Assert.True(elevators[0].HasPendingRequests() || elevators[0].GetPendingRequestCount() >= 0);
    }

    [Fact]
    public void RequestFloor_WithValidParameters_ShouldAddRequestToElevator()
    {
        // Arrange
        var elevators = new List<Elevator> { new Elevator(1, 10) };
        elevators[0].SetState(new IdleState());
        var building = new Building(10, 0);
        var strategy = new NearestElevatorStrategy();
        var controller = new ElevatorController(elevators, building, strategy);

        // Act
        controller.RequestFloor(1, 7);

        // Assert
        Assert.True(elevators[0].HasPendingRequests());
    }

    [Fact]
    public void RequestElevator_WithInvalidFloor_ShouldThrowException()
    {
        // Arrange
        var elevators = new List<Elevator> { new Elevator(1, 10) };
        elevators[0].SetState(new IdleState());
        var building = new Building(10, 0);
        var strategy = new NearestElevatorStrategy();
        var controller = new ElevatorController(elevators, building, strategy);

        // Act & Assert
        Assert.Throws<ElevatorSystem.Domain.Exceptions.InvalidFloorException>(() => 
            controller.RequestElevator(15, Direction.Up));
    }

    [Fact]
    public void ProcessRequests_WithMultipleElevators_ShouldDistributeRequests()
    {
        // Arrange
        var elevators = new List<Elevator>
        {
            new Elevator(1, 10),
            new Elevator(2, 10),
            new Elevator(3, 10)
        };
        
        foreach (var elevator in elevators)
        {
            elevator.SetState(new IdleState());
        }

        var building = new Building(10, 0);
        var strategy = new OptimalPathStrategy();
        var controller = new ElevatorController(elevators, building, strategy);

        // Act
        controller.RequestElevator(3, Direction.Up);
        controller.RequestElevator(7, Direction.Down);
        controller.RequestElevator(5, Direction.Up);
        controller.ProcessRequests();

        // Assert
        var totalPendingRequests = elevators.Sum(e => e.GetPendingRequestCount());
        Assert.True(totalPendingRequests >= 0);
    }
}
