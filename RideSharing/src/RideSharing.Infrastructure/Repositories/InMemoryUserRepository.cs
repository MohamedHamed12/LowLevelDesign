using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;
using RideSharing.Domain.ValueObjects;

namespace RideSharing.Infrastructure.Repositories;

/// <summary>
/// In-memory repository for managing users (riders and drivers)
/// In production, this would be replaced with a database implementation
/// </summary>
public class InMemoryUserRepository
{
    private readonly Dictionary<Guid, User> _users = new();
    private readonly object _lock = new();

    public void Add(User user)
    {
        lock (_lock)
        {
            _users[user.Id] = user;
        }
    }

    public User? GetById(Guid id)
    {
        lock (_lock)
        {
            return _users.TryGetValue(id, out var user) ? user : null;
        }
    }

    public List<Rider> GetAllRiders()
    {
        lock (_lock)
        {
            return _users.Values.OfType<Rider>().ToList();
        }
    }

    public List<Driver> GetAllDrivers()
    {
        lock (_lock)
        {
            return _users.Values.OfType<Driver>().ToList();
        }
    }

    public List<Driver> GetAvailableDrivers()
    {
        lock (_lock)
        {
            return _users.Values
                .OfType<Driver>()
                .Where(d => d.IsAvailable && d.CurrentLocation != null)
                .ToList();
        }
    }

    public Driver? GetDriverByLocation(Location location, double radiusKm)
    {
        lock (_lock)
        {
            return _users.Values
                .OfType<Driver>()
                .Where(d => d.IsAvailable && d.IsNearby(location, radiusKm))
                .FirstOrDefault();
        }
    }
}
