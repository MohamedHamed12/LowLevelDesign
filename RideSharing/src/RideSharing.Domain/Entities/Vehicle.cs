using RideSharing.Domain.Enums;

namespace RideSharing.Domain.Entities;

/// <summary>
/// Represents a vehicle in the system
/// </summary>
public class Vehicle
{
    public Guid Id { get; private set; }
    public string RegistrationNumber { get; private set; }
    public VehicleType VehicleType { get; private set; }
    public string Make { get; private set; }
    public string Model { get; private set; }
    public int Year { get; private set; }
    public string Color { get; private set; }
    public int Capacity { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Vehicle(
        string registrationNumber,
        VehicleType vehicleType,
        string make,
        string model,
        int year,
        string color,
        int capacity)
    {
        Id = Guid.NewGuid();
        RegistrationNumber = registrationNumber ?? throw new ArgumentNullException(nameof(registrationNumber));
        VehicleType = vehicleType;
        Make = make ?? throw new ArgumentNullException(nameof(make));
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Year = year;
        Color = color ?? throw new ArgumentNullException(nameof(color));
        Capacity = capacity > 0 ? capacity : throw new ArgumentException("Capacity must be positive");
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void UpdateDetails(string make, string model, string color)
    {
        Make = make ?? Make;
        Model = model ?? Model;
        Color = color ?? Color;
    }

    public override string ToString()
    {
        return $"{Color} {Make} {Model} ({RegistrationNumber})";
    }
}
