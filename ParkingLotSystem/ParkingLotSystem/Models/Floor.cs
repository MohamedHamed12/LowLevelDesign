using System.Collections.Concurrent;
using ParkingLotSystem.Models.Enums;
using ParkingLotSystem.Models.Spots;

namespace ParkingLotSystem.Models;

public class Floor
{
    private readonly ConcurrentDictionary<SpotType, ConcurrentBag<ParkingSpot>> _spotsByType;

    public int FloorNumber { get; }

    public Floor(int floorNumber, Dictionary<SpotType, int> spotConfiguration)
    {
        FloorNumber = floorNumber;
        _spotsByType = new ConcurrentDictionary<SpotType, ConcurrentBag<ParkingSpot>>();

        InitializeSpots(spotConfiguration);
    }

    private void InitializeSpots(Dictionary<SpotType, int> spotConfiguration)
    {
        foreach (var (spotType, count) in spotConfiguration)
        {
            var spots = new ConcurrentBag<ParkingSpot>();

            for (int i = 0; i < count; i++)
            {
                var spotId = $"F{FloorNumber}-{spotType}-{i + 1:D3}";
                ParkingSpot spot = spotType switch
                {
                    SpotType.Compact => new CompactSpot(spotId),
                    SpotType.Standard => new StandardSpot(spotId),
                    SpotType.Large => new LargeSpot(spotId),
                    SpotType.Electric => new ElectricSpot(spotId),
                    _ => throw new ArgumentException($"Unknown spot type: {spotType}")
                };

                spots.Add(spot);
            }

            _spotsByType[spotType] = spots;
        }
    }

    public ParkingSpot? FindAvailableSpot(SpotType spotType)
    {
        if (!_spotsByType.TryGetValue(spotType, out var spots))
            return null;

        return spots.FirstOrDefault(s => s.IsAvailable());
    }

    public int GetAvailableCount(SpotType spotType)
    {
        if (!_spotsByType.TryGetValue(spotType, out var spots))
            return 0;

        return spots.Count(s => s.IsAvailable());
    }

    public void ReleaseSpot(string spotId)
    {
        foreach (var spots in _spotsByType.Values)
        {
            var spot = spots.FirstOrDefault(s => s.SpotId == spotId);
            spot?.ReleaseSpot();
        }
    }
}
