using ParkingLotSystem.Models;
using ParkingLotSystem.Models.Enums;

namespace ParkingLotSystem.Strategies;

public class HourlyPricingStrategy : IPricingStrategy
{
    private readonly Dictionary<VehicleType, decimal> _hourlyRates;

    public HourlyPricingStrategy()
    {
        _hourlyRates = new Dictionary<VehicleType, decimal>
        {
            { VehicleType.Motorcycle, 2.00m },
            { VehicleType.Car, 5.00m },
            { VehicleType.ElectricCar, 6.00m },
            { VehicleType.Truck, 10.00m }
        };
    }

    public decimal CalculateFee(ParkingTicket ticket, DateTime exitTime)
    {
        var duration = exitTime - ticket.EntryTime;
        var hours = Math.Ceiling(duration.TotalHours);

        if (hours < 1)
            hours = 1; // Minimum 1 hour charge

        var rate = _hourlyRates.GetValueOrDefault(ticket.Vehicle.Type, 5.00m);
        return rate * (decimal)hours;
    }
}
