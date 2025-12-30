using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Enums;
using Xunit;

namespace ElevatorSystem.Tests.Domain;

public class RequestTests
{
    [Fact]
    public void Constructor_ShouldCreateExternalRequest()
    {
        // Arrange & Act
        var request = new Request(5, Direction.Up, RequestType.External);

        // Assert
        Assert.Equal(5, request.SourceFloor);
        Assert.Equal(Direction.Up, request.Direction);
        Assert.Equal(RequestType.External, request.RequestType);
        Assert.Null(request.DestinationFloor);
    }

    [Fact]
    public void Constructor_ShouldCreateInternalRequest()
    {
        // Arrange & Act
        var request = new Request(3, Direction.Up, RequestType.Internal, destinationFloor: 7);

        // Assert
        Assert.Equal(3, request.SourceFloor);
        Assert.Equal(7, request.DestinationFloor);
        Assert.Equal(Direction.Up, request.Direction);
        Assert.Equal(RequestType.Internal, request.RequestType);
    }

    [Fact]
    public void CalculatePriority_ShouldSetCorrectPriority()
    {
        // Arrange
        var request = new Request(5, Direction.Up, RequestType.External);

        // Act
        request.CalculatePriority(currentFloor: 2);

        // Assert
        Assert.Equal(3, request.Priority); // Distance from floor 2 to floor 5
    }

    [Fact]
    public void CompareTo_ShouldOrderByPriority()
    {
        // Arrange
        var request1 = new Request(5, Direction.Up, RequestType.External);
        var request2 = new Request(3, Direction.Up, RequestType.External);
        
        request1.SetPriority(5);
        request2.SetPriority(3);

        // Act
        int comparison = request1.CompareTo(request2);

        // Assert
        Assert.True(comparison > 0); // request1 has lower priority (higher value)
    }

    [Fact]
    public void Equals_ShouldReturnTrueForIdenticalRequests()
    {
        // Arrange
        var request1 = new Request(5, Direction.Up, RequestType.External);
        var request2 = new Request(5, Direction.Up, RequestType.External);

        // Act & Assert
        Assert.True(request1.Equals(request2));
    }

    [Fact]
    public void Equals_ShouldReturnFalseForDifferentRequests()
    {
        // Arrange
        var request1 = new Request(5, Direction.Up, RequestType.External);
        var request2 = new Request(3, Direction.Down, RequestType.External);

        // Act & Assert
        Assert.False(request1.Equals(request2));
    }
}
