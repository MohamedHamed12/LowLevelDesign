using ElevatorSystem.Application.States;
using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Enums;
using Xunit;

namespace ElevatorSystem.Tests.Application;

public class ElevatorStateTests
{
    [Fact]
    public void IdleState_CanAcceptRequest_ShouldReturnTrue()
    {
        // Arrange
        var state = new IdleState();

        // Act & Assert
        Assert.True(state.CanAcceptRequest());
    }

    [Fact]
    public void MovingUpState_CanAcceptRequest_ShouldReturnTrue()
    {
        // Arrange
        var state = new MovingUpState();

        // Act & Assert
        Assert.True(state.CanAcceptRequest());
    }

    [Fact]
    public void MovingDownState_CanAcceptRequest_ShouldReturnTrue()
    {
        // Arrange
        var state = new MovingDownState();

        // Act & Assert
        Assert.True(state.CanAcceptRequest());
    }

    [Fact]
    public void MaintenanceState_CanAcceptRequest_ShouldReturnFalse()
    {
        // Arrange
        var state = new MaintenanceState();

        // Act & Assert
        Assert.False(state.CanAcceptRequest());
    }

    [Fact]
    public void IdleState_Handle_WithNoRequests_ShouldRemainIdle()
    {
        // Arrange
        var elevator = new Elevator(1, 10);
        var state = new IdleState();
        elevator.SetState(state);

        // Act
        state.Handle(elevator);

        // Assert
        Assert.Equal("Idle", elevator.CurrentStateName);
    }

    [Fact]
    public void IdleState_Handle_WithUpwardRequest_ShouldTransitionToMovingUp()
    {
        // Arrange
        var elevator = new Elevator(1, 10);
        var state = new IdleState();
        elevator.SetState(state);
        elevator.AddRequest(5);

        // Act
        state.Handle(elevator);

        // Assert
        Assert.Equal("Moving Up", elevator.CurrentStateName);
    }

    [Fact]
    public void MovingUpState_StateName_ShouldBeCorrect()
    {
        // Arrange
        var state = new MovingUpState();

        // Act & Assert
        Assert.Equal("Moving Up", state.StateName);
    }

    [Fact]
    public void MovingDownState_StateName_ShouldBeCorrect()
    {
        // Arrange
        var state = new MovingDownState();

        // Act & Assert
        Assert.Equal("Moving Down", state.StateName);
    }
}
