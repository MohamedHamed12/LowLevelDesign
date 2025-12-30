using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;
using RideSharing.Domain.Interfaces;
using RideSharing.Domain.ValueObjects;
using RideSharing.Application.Services;
using RideSharing.Application.Strategies;
using RideSharing.Application.Observers;
using RideSharing.Infrastructure.Services;
using RideSharing.Infrastructure.Repositories;

namespace RideSharing.Console;

class Program
{
    private static InMemoryUserRepository _userRepository = null!;
    private static InMemoryTripRepository _tripRepository = null!;
    private static TripService _tripService = null!;
    private static PaymentService _paymentService = null!;
    private static RatingService _ratingService = null!;
    private static INotificationService _notificationService = null!;

    static async Task Main(string[] args)
    {
        System.Console.Clear();
        DisplayBanner();

        InitializeServices();
        SeedData();

        while (true)
        {
            DisplayMenu();
            var choice = System.Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        await SimulateRideScenario();
                        break;
                    case "2":
                        ViewAllDrivers();
                        break;
                    case "3":
                        ViewAllRiders();
                        break;
                    case "4":
                        ViewAllTrips();
                        break;
                    case "5":
                        await CreateNewRider();
                        break;
                    case "6":
                        await CreateNewDriver();
                        break;
                    case "7":
                        GetFareEstimate();
                        break;
                    case "8":
                        await RunFullSimulation();
                        break;
                    case "0":
                        System.Console.WriteLine("\n👋 Thank you for using RideSharing Service!");
                        return;
                    default:
                        System.Console.WriteLine("❌ Invalid choice. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"\n❌ Error: {ex.Message}");
                System.Console.ResetColor();
            }

            System.Console.WriteLine("\nPress any key to continue...");
            System.Console.ReadKey();
        }
    }

    static void DisplayBanner()
    {
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║                                                            ║");
        System.Console.WriteLine("║          🚗  RIDE-SHARING SERVICE SIMULATION  🚕          ║");
        System.Console.WriteLine("║                                                            ║");
        System.Console.WriteLine("║     Advanced Low-Level Design Implementation (.NET 9)     ║");
        System.Console.WriteLine("║                                                            ║");
        System.Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        System.Console.ResetColor();
        System.Console.WriteLine();
    }

    static void DisplayMenu()
    {
        System.Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║                        MAIN MENU                           ║");
        System.Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
        System.Console.WriteLine("║  1. 🚗 Simulate a Complete Ride                           ║");
        System.Console.WriteLine("║  2. 👥 View All Drivers                                   ║");
        System.Console.WriteLine("║  3. 👤 View All Riders                                    ║");
        System.Console.WriteLine("║  4. 📋 View All Trips                                     ║");
        System.Console.WriteLine("║  5. ➕ Create New Rider                                   ║");
        System.Console.WriteLine("║  6. ➕ Create New Driver                                  ║");
        System.Console.WriteLine("║  7. 💰 Get Fare Estimate                                  ║");
        System.Console.WriteLine("║  8. 🎬 Run Full Simulation (Multiple Rides)               ║");
        System.Console.WriteLine("║  0. 🚪 Exit                                               ║");
        System.Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        System.Console.Write("\nEnter your choice: ");
    }

    static void InitializeServices()
    {
        _userRepository = new InMemoryUserRepository();
        _tripRepository = new InMemoryTripRepository();
        _notificationService = new ConsoleNotificationService();
        _paymentService = new PaymentService(new MockPaymentGateway());
        _ratingService = new RatingService();

        // Use standard pricing strategy
        var pricingStrategy = new StandardPricingStrategy();
        var driverMatchingStrategy = new NearestDriverStrategy();
        var drivers = new List<Driver>();

        _tripService = new TripService(driverMatchingStrategy, pricingStrategy, drivers);
    }

    static void SeedData()
    {
        // Create sample riders
        var rider1 = new Rider("Alice Johnson", "alice@example.com", "+1234567890", "hash1");
        rider1.AddToWallet(new Money(100m, "USD"));
        rider1.UpdateLocation(new Location(37.7749, -122.4194)); // San Francisco
        _userRepository.Add(rider1);

        var rider2 = new Rider("Bob Smith", "bob@example.com", "+1234567891", "hash2");
        rider2.AddToWallet(new Money(150m, "USD"));
        rider2.UpdateLocation(new Location(37.7849, -122.4094)); // SF - different location
        _userRepository.Add(rider2);

        // Create sample drivers
        var driver1 = new Driver("John Doe", "john@example.com", "+1234567892", "hash3", "DL12345");
        var vehicle1 = new Vehicle("ABC123", VehicleType.Sedan, "Toyota", "Camry", 2022, "Black", 4);
        driver1.AssignVehicle(vehicle1);
        driver1.VerifyDocuments();
        driver1.UpdateLocation(new Location(37.7750, -122.4195));
        driver1.GoOnline();
        _userRepository.Add(driver1);

        var driver2 = new Driver("Jane Smith", "jane@example.com", "+1234567893", "hash4", "DL67890");
        var vehicle2 = new Vehicle("XYZ789", VehicleType.SUV, "Honda", "CR-V", 2023, "White", 5);
        driver2.AssignVehicle(vehicle2);
        driver2.VerifyDocuments();
        driver2.UpdateLocation(new Location(37.7650, -122.4294));
        driver2.GoOnline();
        _userRepository.Add(driver2);

        var driver3 = new Driver("Mike Wilson", "mike@example.com", "+1234567894", "hash5", "DL11111");
        var vehicle3 = new Vehicle("DEF456", VehicleType.Mini, "Hyundai", "i10", 2021, "Red", 4);
        driver3.AssignVehicle(vehicle3);
        driver3.VerifyDocuments();
        driver3.UpdateLocation(new Location(37.7850, -122.4095));
        driver3.GoOnline();
        _userRepository.Add(driver3);

        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine("✅ Sample data initialized:");
        System.Console.WriteLine($"   - {_userRepository.GetAllRiders().Count} Riders");
        System.Console.WriteLine($"   - {_userRepository.GetAllDrivers().Count} Drivers");
        System.Console.ResetColor();
    }

    static async Task SimulateRideScenario()
    {
        System.Console.Clear();
        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.WriteLine("\n🚗 SIMULATING COMPLETE RIDE SCENARIO");
        System.Console.WriteLine("════════════════════════════════════\n");
        System.Console.ResetColor();

        var riders = _userRepository.GetAllRiders();
        var rider = riders.First();

        System.Console.WriteLine($"👤 Rider: {rider.Name}");
        System.Console.WriteLine($"💰 Wallet Balance: {rider.WalletBalance}");

        // Define trip locations
        var pickup = new Location(37.7749, -122.4194);
        var dropoff = new Location(37.8049, -122.4394);

        System.Console.WriteLine($"\n📍 Pickup: {pickup}");
        System.Console.WriteLine($"📍 Dropoff: {dropoff}");
        System.Console.WriteLine($"📏 Distance: {pickup.DistanceTo(dropoff):F2} km");

        // Step 1: Request trip
        System.Console.WriteLine("\n➡️  Step 1: Requesting trip...");
        await Task.Delay(500);

        var availableDrivers = _userRepository.GetAvailableDrivers();
        var tripService = new TripService(
            new NearestDriverStrategy(),
            new StandardPricingStrategy(),
            availableDrivers);

        var trip = tripService.RequestTrip(rider, pickup, dropoff, VehicleType.Sedan);
        _tripRepository.Add(trip);

        // Add notification observer
        var observer = new TripNotificationObserver(_notificationService);
        trip.AddObserver(observer);

        System.Console.WriteLine($"✅ Trip requested (ID: {trip.Id.ToString().Substring(0, 8)})");
        System.Console.WriteLine($"💵 Estimated Fare: {trip.EstimatedFare}");

        // Step 2: Assign driver
        System.Console.WriteLine("\n➡️  Step 2: Finding nearest driver...");
        await Task.Delay(800);

        try
        {
            tripService.AssignDriver(trip);
            System.Console.WriteLine($"✅ Driver assigned: {trip.Driver!.Name}");
            System.Console.WriteLine($"🚗 Vehicle: {trip.Driver.Vehicle!.Make} {trip.Driver.Vehicle.Model} ({trip.Driver.Vehicle.RegistrationNumber})");
            System.Console.WriteLine($"⭐ Driver Rating: {trip.Driver.Rating}");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ {ex.Message}");
            return;
        }

        // Step 3: Driver en route
        System.Console.WriteLine("\n➡️  Step 3: Driver en route to pickup...");
        await Task.Delay(1000);
        trip.MarkDriverEnRoute();

        // Step 4: Driver arrived
        System.Console.WriteLine("\n➡️  Step 4: Driver arrived at pickup...");
        await Task.Delay(1000);
        trip.MarkDriverArrived();

        // Step 5: Start trip
        System.Console.WriteLine("\n➡️  Step 5: Starting trip...");
        await Task.Delay(800);
        tripService.StartTrip(trip);

        // Step 6: Complete trip
        System.Console.WriteLine("\n➡️  Step 6: Trip in progress...");
        await Task.Delay(2000);
        System.Console.WriteLine("\n➡️  Step 7: Completing trip...");
        await Task.Delay(500);
        tripService.CompleteTrip(trip, dropoff);

        System.Console.WriteLine($"\n✅ Trip completed!");
        System.Console.WriteLine($"💵 Actual Fare: {trip.ActualFare}");
        System.Console.WriteLine($"📏 Distance: {trip.Distance:F2} km");
        System.Console.WriteLine($"⏱️  Duration: {trip.Duration} minutes");

        // Step 8: Process payment
        System.Console.WriteLine("\n➡️  Step 8: Processing payment...");
        var payment = new Payment(trip, trip.ActualFare!, PaymentMethod.Card);
        payment.AddObserver(observer);

        try
        {
            var paymentResult = await _paymentService.ProcessPaymentAsync(trip, PaymentMethod.Card);
            System.Console.WriteLine($"✅ Payment processed: {paymentResult.TransactionId}");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ Payment failed: {ex.Message}");
        }

        // Step 9: Ratings
        System.Console.WriteLine("\n➡️  Step 9: Exchanging ratings...");
        await Task.Delay(500);

        var riderRating = new RatingScore(4.5);
        var driverRating = new RatingScore(5.0);

        _ratingService.RateDriver(trip, rider, driverRating, "Excellent service!");
        _ratingService.RateRider(trip, trip.Driver!, riderRating, "Great passenger!");

        System.Console.WriteLine($"⭐ Rider rated driver: {driverRating}");
        System.Console.WriteLine($"⭐ Driver rated rider: {riderRating}");

        // Display summary
        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine("\n✅ RIDE COMPLETED SUCCESSFULLY!");
        System.Console.WriteLine("═══════════════════════════════");
        System.Console.WriteLine($"Total Trips in System: {_tripRepository.GetTotalTripsCount()}");
        System.Console.WriteLine($"Completed Trips: {_tripRepository.GetCompletedTripsCount()}");
        System.Console.ResetColor();
    }

    static void ViewAllDrivers()
    {
        System.Console.Clear();
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine("\n👥 ALL DRIVERS");
        System.Console.WriteLine("═══════════════════════════════════════════════════════════\n");
        System.Console.ResetColor();

        var drivers = _userRepository.GetAllDrivers();

        foreach (var driver in drivers)
        {
            var status = driver.IsAvailable ? "🟢 Online" : "🔴 Offline";
            System.Console.WriteLine($"{status} {driver.Name}");
            System.Console.WriteLine($"   📧 {driver.Email} | 📱 {driver.PhoneNumber}");
            System.Console.WriteLine($"   🚗 {driver.Vehicle?.Make} {driver.Vehicle?.Model} ({driver.Vehicle?.VehicleType})");
            System.Console.WriteLine($"   ⭐ Rating: {driver.Rating} | 🚕 Trips: {driver.TotalTrips}");
            System.Console.WriteLine($"   💰 Earnings: {driver.TotalEarnings}");
            if (driver.CurrentLocation != null)
                System.Console.WriteLine($"   📍 Location: {driver.CurrentLocation}");
            System.Console.WriteLine();
        }

        System.Console.WriteLine($"Total Drivers: {drivers.Count}");
        System.Console.WriteLine($"Available: {drivers.Count(d => d.IsAvailable)}");
    }

    static void ViewAllRiders()
    {
        System.Console.Clear();
        System.Console.ForegroundColor = ConsoleColor.Magenta;
        System.Console.WriteLine("\n👤 ALL RIDERS");
        System.Console.WriteLine("═══════════════════════════════════════════════════════════\n");
        System.Console.ResetColor();

        var riders = _userRepository.GetAllRiders();

        foreach (var rider in riders)
        {
            System.Console.WriteLine($"👤 {rider.Name}");
            System.Console.WriteLine($"   📧 {rider.Email} | 📱 {rider.PhoneNumber}");
            System.Console.WriteLine($"   💰 Wallet: {rider.WalletBalance}");
            System.Console.WriteLine($"   ⭐ Rating: {rider.Rating} | 🚕 Trips: {rider.TotalTrips}");
            System.Console.WriteLine($"   🚗 Preferred: {rider.PreferredVehicleType}");
            System.Console.WriteLine();
        }

        System.Console.WriteLine($"Total Riders: {riders.Count}");
    }

    static void ViewAllTrips()
    {
        System.Console.Clear();
        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.WriteLine("\n📋 ALL TRIPS");
        System.Console.WriteLine("═══════════════════════════════════════════════════════════\n");
        System.Console.ResetColor();

        var trips = _tripRepository.GetAll();

        if (trips.Count == 0)
        {
            System.Console.WriteLine("No trips found.");
            return;
        }

        foreach (var trip in trips)
        {
            var statusColor = trip.Status switch
            {
                TripStatus.Completed => ConsoleColor.Green,
                TripStatus.Cancelled => ConsoleColor.Red,
                TripStatus.InProgress => ConsoleColor.Yellow,
                _ => ConsoleColor.White
            };

            System.Console.ForegroundColor = statusColor;
            System.Console.WriteLine($"🚗 Trip {trip.Id.ToString().Substring(0, 8)} - {trip.Status}");
            System.Console.ResetColor();

            System.Console.WriteLine($"   👤 Rider: {trip.Rider.Name}");
            System.Console.WriteLine($"   👥 Driver: {trip.Driver?.Name ?? "Not assigned"}");
            System.Console.WriteLine($"   🚙 Vehicle Type: {trip.RequestedVehicleType}");
            System.Console.WriteLine($"   📅 Requested: {trip.RequestedAt:g}");

            if (trip.EstimatedFare != null)
                System.Console.WriteLine($"   💵 Estimated Fare: {trip.EstimatedFare}");

            if (trip.ActualFare != null)
                System.Console.WriteLine($"   💰 Actual Fare: {trip.ActualFare}");

            if (trip.Distance.HasValue)
                System.Console.WriteLine($"   📏 Distance: {trip.Distance:F2} km");

            if (trip.Duration.HasValue)
                System.Console.WriteLine($"   ⏱️  Duration: {trip.Duration} minutes");

            System.Console.WriteLine();
        }

        System.Console.WriteLine($"Total Trips: {trips.Count}");
        System.Console.WriteLine($"Completed: {trips.Count(t => t.Status == TripStatus.Completed)}");
        System.Console.WriteLine($"In Progress: {trips.Count(t => t.Status == TripStatus.InProgress)}");
        System.Console.WriteLine($"Cancelled: {trips.Count(t => t.Status == TripStatus.Cancelled)}");
    }

    static async Task CreateNewRider()
    {
        System.Console.Clear();
        System.Console.WriteLine("\n➕ CREATE NEW RIDER\n");

        System.Console.Write("Name: ");
        var name = System.Console.ReadLine() ?? "";

        System.Console.Write("Email: ");
        var email = System.Console.ReadLine() ?? "";

        System.Console.Write("Phone: ");
        var phone = System.Console.ReadLine() ?? "";

        var rider = new Rider(name, email, phone, "hash");
        rider.AddToWallet(new Money(100m, "USD"));
        rider.UpdateLocation(new Location(37.7749 + Random.Shared.NextDouble() * 0.1, -122.4194 + Random.Shared.NextDouble() * 0.1));

        _userRepository.Add(rider);

        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine($"\n✅ Rider created successfully!");
        System.Console.WriteLine($"   ID: {rider.Id}");
        System.Console.WriteLine($"   Wallet: {rider.WalletBalance}");
        System.Console.ResetColor();

        await Task.CompletedTask;
    }

    static async Task CreateNewDriver()
    {
        System.Console.Clear();
        System.Console.WriteLine("\n➕ CREATE NEW DRIVER\n");

        System.Console.Write("Name: ");
        var name = System.Console.ReadLine() ?? "";

        System.Console.Write("Email: ");
        var email = System.Console.ReadLine() ?? "";

        System.Console.Write("Phone: ");
        var phone = System.Console.ReadLine() ?? "";

        System.Console.Write("License Number: ");
        var license = System.Console.ReadLine() ?? "";

        System.Console.Write("Vehicle Registration: ");
        var registration = System.Console.ReadLine() ?? "";

        System.Console.Write("Vehicle Type (1=Bike, 2=Auto, 3=Mini, 4=Sedan, 5=SUV, 6=Luxury): ");
        var vehicleTypeChoice = System.Console.ReadLine();
        var vehicleType = vehicleTypeChoice switch
        {
            "1" => VehicleType.Bike,
            "2" => VehicleType.Auto,
            "3" => VehicleType.Mini,
            "5" => VehicleType.SUV,
            "6" => VehicleType.Luxury,
            _ => VehicleType.Sedan
        };

        var driver = new Driver(name, email, phone, "hash", license);
        var vehicle = new Vehicle(registration, vehicleType, "Generic", "Model", 2023, "Black", 4);

        driver.AssignVehicle(vehicle);
        driver.VerifyDocuments();
        driver.UpdateLocation(new Location(37.7749 + Random.Shared.NextDouble() * 0.1, -122.4194 + Random.Shared.NextDouble() * 0.1));
        driver.GoOnline();

        _userRepository.Add(driver);

        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine($"\n✅ Driver created and set online!");
        System.Console.WriteLine($"   ID: {driver.Id}");
        System.Console.WriteLine($"   Vehicle: {vehicle.VehicleType}");
        System.Console.ResetColor();

        await Task.CompletedTask;
    }

    static void GetFareEstimate()
    {
        System.Console.Clear();
        System.Console.WriteLine("\n💰 FARE ESTIMATE\n");

        System.Console.Write("Enter distance (km): ");
        var distance = double.Parse(System.Console.ReadLine() ?? "5");

        System.Console.Write("Vehicle Type (1=Bike, 2=Auto, 3=Mini, 4=Sedan, 5=SUV, 6=Luxury): ");
        var choice = System.Console.ReadLine();
        var vehicleType = choice switch
        {
            "1" => VehicleType.Bike,
            "2" => VehicleType.Auto,
            "3" => VehicleType.Mini,
            "5" => VehicleType.SUV,
            "6" => VehicleType.Luxury,
            _ => VehicleType.Sedan
        };

        var duration = (int)(distance / 40.0 * 60); // Estimate based on 40 km/h

        System.Console.WriteLine("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        System.Console.WriteLine("STANDARD PRICING:");
        var standardFare = new StandardPricingStrategy().EstimateFare(distance, duration, vehicleType);
        System.Console.WriteLine($"  Fare: {standardFare}");

        System.Console.WriteLine("\nSURGE PRICING (1.5x):");
        var surgeFare = new SurgePricingStrategy(1.5).EstimateFare(distance, duration, vehicleType);
        System.Console.WriteLine($"  Fare: {surgeFare}");
        System.Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

        // Show detailed breakdown
        var fareObj = Fare.Calculate(vehicleType, distance, TimeSpan.FromMinutes(duration));
        System.Console.WriteLine("\n" + fareObj.GetBreakdown());
    }

    static async Task RunFullSimulation()
    {
        System.Console.Clear();
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine("\n🎬 RUNNING FULL SIMULATION");
        System.Console.WriteLine("═══════════════════════════════════════════════════════════\n");
        System.Console.ResetColor();

        System.Console.WriteLine("This will simulate multiple concurrent rides...\n");

        var riders = _userRepository.GetAllRiders();
        var tasks = new List<Task>();

        foreach (var rider in riders.Take(2))
        {
            tasks.Add(SimulateRideForRider(rider));
        }

        await Task.WhenAll(tasks);

        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine("\n\n✅ FULL SIMULATION COMPLETED!");
        System.Console.WriteLine($"Total Trips: {_tripRepository.GetTotalTripsCount()}");
        System.Console.WriteLine($"Completed: {_tripRepository.GetCompletedTripsCount()}");
        System.Console.ResetColor();
    }

    static async Task SimulateRideForRider(Rider rider)
    {
        var pickup = new Location(37.7749 + Random.Shared.NextDouble() * 0.05, -122.4194 + Random.Shared.NextDouble() * 0.05);
        var dropoff = new Location(37.7849 + Random.Shared.NextDouble() * 0.05, -122.4094 + Random.Shared.NextDouble() * 0.05);

        var availableDrivers = _userRepository.GetAvailableDrivers();
        var tripService = new TripService(
            new NearestDriverStrategy(),
            new StandardPricingStrategy(),
            availableDrivers);

        var trip = tripService.RequestTrip(rider, pickup, dropoff, VehicleType.Sedan);
        _tripRepository.Add(trip);

        var observer = new TripNotificationObserver(_notificationService);
        trip.AddObserver(observer);

        await Task.Delay(500);

        try
        {
            tripService.AssignDriver(trip);
            await Task.Delay(300);

            trip.MarkDriverEnRoute();
            await Task.Delay(300);

            trip.MarkDriverArrived();
            await Task.Delay(300);

            tripService.StartTrip(trip);
            await Task.Delay(500);

            tripService.CompleteTrip(trip, dropoff);

            await _paymentService.ProcessPaymentAsync(trip, PaymentMethod.Card);

            _ratingService.RateDriver(trip, rider, new RatingScore(4.5), "Good ride");
            _ratingService.RateRider(trip, trip.Driver!, new RatingScore(5.0), "Great passenger");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ Simulation error for {rider.Name}: {ex.Message}");
        }
    }
}
