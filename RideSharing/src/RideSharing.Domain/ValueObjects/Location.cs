namespace RideSharing.Domain.ValueObjects;

/// <summary>
/// Represents a geographic location with latitude and longitude
/// Value Object - immutable
/// </summary>
public record Location
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? Address { get; init; }
    public DateTime Timestamp { get; init; }

    public Location(double latitude, double longitude, string? address = null)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90", nameof(latitude));

        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180", nameof(longitude));

        Latitude = latitude;
        Longitude = longitude;
        Address = address;
        Timestamp = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates distance to another location using Haversine formula
    /// </summary>
    /// <param name="other">Target location</param>
    /// <returns>Distance in kilometers</returns>
    public double DistanceTo(Location other)
    {
        const double earthRadiusKm = 6371.0;

        var dLat = DegreesToRadians(other.Latitude - Latitude);
        var dLon = DegreesToRadians(other.Longitude - Longitude);

        var lat1 = DegreesToRadians(Latitude);
        var lat2 = DegreesToRadians(other.Latitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    /// <summary>
    /// Checks if another location is within specified radius
    /// </summary>
    public bool IsNearby(Location other, double radiusKm)
    {
        return DistanceTo(other) <= radiusKm;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    public override string ToString()
    {
        return Address ?? $"({Latitude:F6}, {Longitude:F6})";
    }
}
