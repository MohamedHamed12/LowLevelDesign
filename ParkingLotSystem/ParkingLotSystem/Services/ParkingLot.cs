using System.Collections.Concurrent;
using ParkingLotSystem.Models;
using ParkingLotSystem.Models.Enums;
using ParkingLotSystem.Models.Vehicles;
using ParkingLotSystem.Strategies;

namespace ParkingLotSystem.Services;

public class ParkingLot
{
    private readonly List<Floor> _floors;
    private readonly ConcurrentDictionary<string, ParkingTicket> _activeTickets;
    private readonly IPricingStrategy _pricingStrategy;

    public string Name { get; }

    public ParkingLot(string name, int numFloors, Dictionary<SpotType, int> spotsPerFloor, IPricingStrategy? pricingStrategy = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _floors = new List<Floor>();
        _activeTickets = new ConcurrentDictionary<string, ParkingTicket>();
        _pricingStrategy = pricingStrategy ?? new HourlyPricingStrategy();

        for (int i = 0; i < numFloors; i++)
        {
            _floors.Add(new Floor(i + 1, spotsPerFloor));
        }
    }

    public ParkingTicket? ParkVehicle(Vehicle vehicle)
    {
        if (vehicle == null)
            throw new ArgumentNullException(nameof(vehicle));

        var requiredSpotType = vehicle.GetRequiredSpotType();
        var spot = FindAvailableSpot(requiredSpotType);

        if (spot == null)
        {
            Console.WriteLine($"No available {requiredSpotType} spots for {vehicle}");
            return null;
        }

        if (!spot.OccupySpot(vehicle))
        {
            Console.WriteLine($"Failed to occupy spot {spot.SpotId}");
            return null;
        }

        var ticket = new ParkingTicket(vehicle, spot);
        _activeTickets[ticket.TicketId] = ticket;

        Console.WriteLine($"✓ Parked {vehicle} at {spot.SpotId} - Ticket: {ticket.TicketId}");
        return ticket;
    }

    public PaymentReceipt? ExitVehicle(string ticketId)
    {
        if (!_activeTickets.TryRemove(ticketId, out var ticket))
        {
            Console.WriteLine($"Invalid or expired ticket: {ticketId}");
            return null;
        }

        var exitTime = DateTime.UtcNow;
        var amount = _pricingStrategy.CalculateFee(ticket, exitTime);

        ticket.Spot.ReleaseSpot();

        var receipt = new PaymentReceipt(ticket, exitTime, amount);
        Console.WriteLine($"✓ {ticket.Vehicle} exited from {ticket.Spot.SpotId} - Fee: ${amount:F2}");

        return receipt;
    }

    public Dictionary<SpotType, int> GetAvailability()
    {
        var availability = new Dictionary<SpotType, int>();

        foreach (SpotType spotType in Enum.GetValues<SpotType>())
        {
            availability[spotType] = _floors.Sum(f => f.GetAvailableCount(spotType));
        }

        return availability;
    }

    private ParkingSpot? FindAvailableSpot(SpotType spotType)
    {
        foreach (var floor in _floors)
        {
            var spot = floor.FindAvailableSpot(spotType);
            if (spot != null)
                return spot;
        }

        return null;
    }

    public void DisplayAvailability()
    {
        Console.WriteLine($"\n=== {Name} - Availability ===");
        var availability = GetAvailability();
        foreach (var (spotType, count) in availability)
        {
            Console.WriteLine($"{spotType,-10}: {count} available");
        }
        Console.WriteLine($"Active Tickets: {_activeTickets.Count}");
    }
}
