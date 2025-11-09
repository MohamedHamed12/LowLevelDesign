using ParkingLotSystem.Models;

namespace ParkingLotSystem.Strategies;

public interface IPricingStrategy
{
    decimal CalculateFee(ParkingTicket ticket, DateTime exitTime);
}
