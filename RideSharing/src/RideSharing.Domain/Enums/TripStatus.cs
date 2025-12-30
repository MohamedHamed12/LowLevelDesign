namespace RideSharing.Domain.Enums;

/// <summary>
/// Represents the current status of a trip
/// </summary>
public enum TripStatus
{
    /// <summary>
    /// Trip has been requested by rider
    /// </summary>
    Requested,

    /// <summary>
    /// Driver has accepted the trip
    /// </summary>
    Accepted,

    /// <summary>
    /// Driver is en route to pickup location
    /// </summary>
    DriverEnRoute,

    /// <summary>
    /// Driver has arrived at pickup location
    /// </summary>
    Arrived,

    /// <summary>
    /// Rider has been picked up
    /// </summary>
    PickedUp,

    /// <summary>
    /// Trip is in progress
    /// </summary>
    InProgress,

    /// <summary>
    /// Trip has been completed successfully
    /// </summary>
    Completed,

    /// <summary>
    /// Trip has been cancelled
    /// </summary>
    Cancelled
}
