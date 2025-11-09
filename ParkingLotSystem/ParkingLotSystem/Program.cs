using ParkingLotSystem.Models.Enums;
using ParkingLotSystem.Models.Vehicles;
using ParkingLotSystem.Services;
using ParkingLotSystem.Strategies;

namespace ParkingLotSystem;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Parking Lot System Demo ===\n");

        // Configure parking lot with 3 floors
        var spotConfiguration = new Dictionary<SpotType, int>
        {
            { SpotType.Compact, 10 },
            { SpotType.Standard, 20 },
            { SpotType.Large, 5 },
            { SpotType.Electric, 8 }
        };

        var parkingLot = new ParkingLot(
            "Downtown Parking", 
            numFloors: 3, 
            spotsPerFloor: spotConfiguration,
            pricingStrategy: new HourlyPricingStrategy()
        );

        parkingLot.DisplayAvailability();

        // Simulate parking operations
        Console.WriteLine("\n--- Parking Vehicles ---");

        var motorcycle = new Motorcycle("MOTO-123");
        var car1 = new Car("CAR-456");
        var car2 = new Car("CAR-789");
        var electricCar = new ElectricCar("TESLA-001");
        var truck = new Truck("TRUCK-999");

        var ticket1 = parkingLot.ParkVehicle(motorcycle);
        var ticket2 = parkingLot.ParkVehicle(car1);
        var ticket3 = parkingLot.ParkVehicle(car2);
        var ticket4 = parkingLot.ParkVehicle(electricCar);
        var ticket5 = parkingLot.ParkVehicle(truck);

        parkingLot.DisplayAvailability();

        // Simulate some time passing
        Console.WriteLine("\n--- Simulating time passage (2 hours) ---");
        Thread.Sleep(100); // In real scenario, this would be actual time

        // Exit vehicles
        Console.WriteLine("\n--- Exiting Vehicles ---");

        if (ticket1 != null)
            parkingLot.ExitVehicle(ticket1.TicketId);

        if (ticket3 != null)
            parkingLot.ExitVehicle(ticket3.TicketId);

        parkingLot.DisplayAvailability();

        // Try to park when space is available
        Console.WriteLine("\n--- Parking Additional Vehicles ---");
        var car3 = new Car("CAR-111");
        var ticket6 = parkingLot.ParkVehicle(car3);

        parkingLot.DisplayAvailability();

        Console.WriteLine("\n=== Demo Complete ===");
    }
}
