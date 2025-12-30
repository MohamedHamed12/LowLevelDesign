using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Exceptions;
using Xunit;

namespace ElevatorSystem.Tests.Domain;

public class BuildingTests
{
    [Fact]
    public void Constructor_ShouldInitializeBuildingCorrectly()
    {
        // Arrange & Act
        var building = new Building(10, 0);

        // Assert
        Assert.Equal(10, building.TotalFloors);
        Assert.Equal(0, building.MinFloor);
        Assert.Equal(9, building.MaxFloor);
    }

    [Fact]
    public void Constructor_WithNegativeMinFloor_ShouldWork()
    {
        // Arrange & Act
        var building = new Building(15, -2);

        // Assert
        Assert.Equal(15, building.TotalFloors);
        Assert.Equal(-2, building.MinFloor);
        Assert.Equal(12, building.MaxFloor);
    }

    [Fact]
    public void IsValidFloor_ShouldReturnTrueForValidFloors()
    {
        // Arrange
        var building = new Building(10, 0);

        // Act & Assert
        Assert.True(building.IsValidFloor(0));
        Assert.True(building.IsValidFloor(5));
        Assert.True(building.IsValidFloor(9));
    }

    [Fact]
    public void IsValidFloor_ShouldReturnFalseForInvalidFloors()
    {
        // Arrange
        var building = new Building(10, 0);

        // Act & Assert
        Assert.False(building.IsValidFloor(-1));
        Assert.False(building.IsValidFloor(10));
        Assert.False(building.IsValidFloor(100));
    }

    [Fact]
    public void GetFloor_ShouldReturnFloorForValidNumber()
    {
        // Arrange
        var building = new Building(10, 0);

        // Act
        var floor = building.GetFloor(5);

        // Assert
        Assert.NotNull(floor);
        Assert.Equal(5, floor.FloorNumber);
    }

    [Fact]
    public void GetFloor_ShouldThrowForInvalidNumber()
    {
        // Arrange
        var building = new Building(10, 0);

        // Act & Assert
        Assert.Throws<InvalidFloorException>(() => building.GetFloor(15));
    }

    [Fact]
    public void ValidateFloor_ShouldNotThrowForValidFloor()
    {
        // Arrange
        var building = new Building(10, 0);

        // Act & Assert - Should not throw
        building.ValidateFloor(5);
    }

    [Fact]
    public void ValidateFloor_ShouldThrowForInvalidFloor()
    {
        // Arrange
        var building = new Building(10, 0);

        // Act & Assert
        Assert.Throws<InvalidFloorException>(() => building.ValidateFloor(20));
    }
}
