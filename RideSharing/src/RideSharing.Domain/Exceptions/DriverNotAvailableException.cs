namespace RideSharing.Domain.Exceptions;

/// <summary>
/// Exception thrown when no driver is available for a ride request
/// </summary>
public class DriverNotAvailableException : Exception
{
    public Guid RequestId { get; }
    public string VehicleType { get; }

    public DriverNotAvailableException(Guid requestId, string vehicleType)
        : base($"No available driver found for request {requestId} with vehicle type {vehicleType}")
    {
        RequestId = requestId;
        VehicleType = vehicleType;
    }

    public DriverNotAvailableException(string message) : base(message)
    {
        RequestId = Guid.Empty;
        VehicleType = string.Empty;
    }
}
