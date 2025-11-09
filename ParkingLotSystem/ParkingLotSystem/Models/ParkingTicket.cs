using ParkingLotSystem.Models.Spots;
using ParkingLotSystem.Models.Vehicles;

namespace ParkingLotSystem.Models;

public class ParkingTicket
{
    public string TicketId { get; }
    public Vehicle Vehicle { get; }
    public ParkingSpot Spot { get; }
    public DateTime EntryTime { get; }

    public ParkingTicket(Vehicle vehicle, ParkingSpot spot)
    {
        TicketId = Guid.NewGuid().ToString();
        Vehicle = vehicle ?? throw new ArgumentNullException(nameof(vehicle));
        Spot = spot ?? throw new ArgumentNullException(nameof(spot));
        EntryTime = DateTime.UtcNow;
    }

    public TimeSpan GetDuration() => DateTime.UtcNow - EntryTime;

    public override string ToString() 
        => $"Ticket {TicketId}: {Vehicle} at {Spot.SpotId} (Entry: {EntryTime:g})";
}
