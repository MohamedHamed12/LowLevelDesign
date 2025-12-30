namespace ElevatorSystem.Infrastructure.Configuration;

/// <summary>
/// Configuration settings for the elevator system
/// </summary>
public class ElevatorConfiguration
{
    /// <summary>
    /// Number of elevators in the system
    /// </summary>
    public int NumberOfElevators { get; set; } = 3;

    /// <summary>
    /// Total number of floors in the building
    /// </summary>
    public int TotalFloors { get; set; } = 10;

    /// <summary>
    /// Minimum floor number (can be negative for basements)
    /// </summary>
    public int MinFloor { get; set; } = 0;

    /// <summary>
    /// Maximum capacity per elevator
    /// </summary>
    public int ElevatorCapacity { get; set; } = 10;

    /// <summary>
    /// Scheduling strategy to use
    /// Options: Nearest, LeastLoaded, OptimalPath
    /// </summary>
    public string SchedulingStrategy { get; set; } = "OptimalPath";

    /// <summary>
    /// Enable verbose logging
    /// </summary>
    public bool EnableLogging { get; set; } = true;

    /// <summary>
    /// Enable display panel observer
    /// </summary>
    public bool EnableDisplayPanel { get; set; } = false;

    /// <summary>
    /// Enable metrics collection
    /// </summary>
    public bool EnableMetrics { get; set; } = true;

    /// <summary>
    /// Validates the configuration
    /// </summary>
    public void Validate()
    {
        if (NumberOfElevators <= 0)
            throw new ArgumentException("Number of elevators must be positive");

        if (TotalFloors <= 0)
            throw new ArgumentException("Total floors must be positive");

        if (ElevatorCapacity <= 0)
            throw new ArgumentException("Elevator capacity must be positive");

        if (string.IsNullOrWhiteSpace(SchedulingStrategy))
            throw new ArgumentException("Scheduling strategy must be specified");
    }
}
