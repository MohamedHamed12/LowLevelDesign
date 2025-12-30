using ElevatorSystem.Domain.Enums;

namespace ElevatorSystem.Domain.Entities;

/// <summary>
/// Represents an elevator request
/// </summary>
public class Request : IComparable<Request>
{
    /// <summary>
    /// Gets the floor where the request originated
    /// </summary>
    public int SourceFloor { get; }

    /// <summary>
    /// Gets the destination floor (null for external requests)
    /// </summary>
    public int? DestinationFloor { get; }

    /// <summary>
    /// Gets the requested direction
    /// </summary>
    public Direction Direction { get; }

    /// <summary>
    /// Gets the request type (internal or external)
    /// </summary>
    public RequestType RequestType { get; }

    /// <summary>
    /// Gets the timestamp when request was created
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// Gets the request priority (lower is higher priority)
    /// </summary>
    public int Priority { get; private set; }

    public Request(int sourceFloor, Direction direction, RequestType requestType, int? destinationFloor = null)
    {
        SourceFloor = sourceFloor;
        Direction = direction;
        RequestType = requestType;
        DestinationFloor = destinationFloor;
        Timestamp = DateTime.UtcNow;
        Priority = 0; // Default priority
    }

    /// <summary>
    /// Sets the priority of the request
    /// </summary>
    public void SetPriority(int priority)
    {
        Priority = priority;
    }

    /// <summary>
    /// Calculates priority based on distance from current floor
    /// </summary>
    public void CalculatePriority(int currentFloor)
    {
        Priority = Math.Abs(currentFloor - SourceFloor);
    }

    /// <summary>
    /// Compares requests for priority queue ordering
    /// </summary>
    public int CompareTo(Request? other)
    {
        if (other == null) return 1;

        // Lower priority value = higher priority
        int priorityComparison = Priority.CompareTo(other.Priority);
        if (priorityComparison != 0)
            return priorityComparison;

        // If same priority, earlier timestamp is higher priority
        return Timestamp.CompareTo(other.Timestamp);
    }

    public override string ToString()
    {
        return RequestType == RequestType.Internal
            ? $"Internal: Floor {SourceFloor} → {DestinationFloor}"
            : $"External: Floor {SourceFloor} ({Direction})";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Request other) return false;
        
        return SourceFloor == other.SourceFloor &&
               DestinationFloor == other.DestinationFloor &&
               Direction == other.Direction &&
               RequestType == other.RequestType;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SourceFloor, DestinationFloor, Direction, RequestType);
    }
}
