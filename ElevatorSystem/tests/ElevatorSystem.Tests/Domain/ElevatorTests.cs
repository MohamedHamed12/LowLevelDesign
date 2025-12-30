using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Enums;
using Xunit;

namespace ElevatorSystem.Tests.Domain;

public class ElevatorTests
{
    [Fact]
    public void Constructor_ShouldInitializeElevatorCorrectly()
    {
        // Arrange & Act
        var elevator = new Elevator(1, 10, 15);

        // Assert
        Assert.Equal(1, elevator.Id);
        Assert.Equal(10, elevator.TotalFloors);
        Assert.Equal(15, elevator.Capacity);
        Assert.Equal(0, elevator.CurrentFloor);
        Assert.Equal(Direction.None, elevator.Direction);
        Assert.Equal(DoorStatus.Closed, elevator.DoorStatus);
    }

    [Fact]
    public void AddRequest_ShouldAddRequestCorrectly()
    {
        // Arrange
        var elevator = new Elevator(1, 10);

        // Act
        elevator.AddRequest(5);

        // Assert
        Assert.True(elevator.HasPendingRequests());
        Assert.Equal(1, elevator.GetPendingRequestCount());
    }

    [Fact]
    public void MoveUp_ShouldIncrementCurrentFloor()
    {
        // Arrange
        var elevator = new Elevator(1, 10);
        var initialFloor = elevator.CurrentFloor;

        // Act
        elevator.MoveUp();

        // Assert
        Assert.Equal(initialFloor + 1, elevator.CurrentFloor);
        Assert.Equal(Direction.Up, elevator.Direction);
    }

    [Fact]
    public void MoveDown_ShouldDecrementCurrentFloor()
    {
        // Arrange
        var elevator = new Elevator(1, 10);
        elevator.MoveUp(); // Move to floor 1 first
        var currentFloor = elevator.CurrentFloor;

        // Act
        elevator.MoveDown();

        // Assert
        Assert.Equal(currentFloor - 1, elevator.CurrentFloor);
        Assert.Equal(Direction.Down, elevator.Direction);
    }

    [Fact]
    public void DistanceToFloor_ShouldCalculateCorrectly()
    {
        // Arrange
        var elevator = new Elevator(1, 10);
        // Elevator is at floor 0

        // Act & Assert
        Assert.Equal(5, elevator.DistanceToFloor(5));
        Assert.Equal(0, elevator.DistanceToFloor(0));
    }

    [Fact]
    public void GetNextTargetFloor_ShouldReturnNullWhenNoRequests()
    {
        // Arrange
        var elevator = new Elevator(1, 10);

        // Act
        var nextFloor = elevator.GetNextTargetFloor();

        // Assert
        Assert.Null(nextFloor);
    }
}
