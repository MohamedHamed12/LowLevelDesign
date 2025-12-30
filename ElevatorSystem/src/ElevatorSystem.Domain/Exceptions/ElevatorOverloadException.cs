namespace ElevatorSystem.Domain.Exceptions;

/// <summary>
/// Exception thrown when elevator is overloaded
/// </summary>
public class ElevatorOverloadException : Exception
{
    public int MaxCapacity { get; }
    public int CurrentLoad { get; }

    public ElevatorOverloadException(int currentLoad, int maxCapacity)
        : base($"Elevator is overloaded. Current: {currentLoad}, Maximum: {maxCapacity}.")
    {
        CurrentLoad = currentLoad;
        MaxCapacity = maxCapacity;
    }
}
