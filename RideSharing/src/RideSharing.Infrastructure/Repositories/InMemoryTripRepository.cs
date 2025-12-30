using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;

namespace RideSharing.Infrastructure.Repositories;

/// <summary>
/// In-memory repository for managing trips
/// </summary>
public class InMemoryTripRepository
{
    private readonly Dictionary<Guid, Trip> _trips = new();
    private readonly object _lock = new();

    public void Add(Trip trip)
    {
        lock (_lock)
        {
            _trips[trip.Id] = trip;
        }
    }

    public Trip? GetById(Guid id)
    {
        lock (_lock)
        {
            return _trips.TryGetValue(id, out var trip) ? trip : null;
        }
    }

    public List<Trip> GetAll()
    {
        lock (_lock)
        {
            return _trips.Values.ToList();
        }
    }

    public List<Trip> GetByRider(Rider rider)
    {
        lock (_lock)
        {
            return _trips.Values
                .Where(t => t.Rider.Id == rider.Id)
                .OrderByDescending(t => t.RequestedAt)
                .ToList();
        }
    }

    public List<Trip> GetByDriver(Driver driver)
    {
        lock (_lock)
        {
            return _trips.Values
                .Where(t => t.Driver?.Id == driver.Id)
                .OrderByDescending(t => t.RequestedAt)
                .ToList();
        }
    }

    public List<Trip> GetByStatus(TripStatus status)
    {
        lock (_lock)
        {
            return _trips.Values
                .Where(t => t.Status == status)
                .ToList();
        }
    }

    public int GetTotalTripsCount()
    {
        lock (_lock)
        {
            return _trips.Count;
        }
    }

    public int GetCompletedTripsCount()
    {
        lock (_lock)
        {
            return _trips.Values.Count(t => t.Status == TripStatus.Completed);
        }
    }
}
