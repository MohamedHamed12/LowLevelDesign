using ElevatorSystem.Application.Factories;
using ElevatorSystem.Application.Observers;
using ElevatorSystem.Application.Services;
using ElevatorSystem.Application.Strategies;
using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Enums;
using ElevatorSystem.Domain.Interfaces;
using ElevatorSystem.Infrastructure.Configuration;
using ElevatorSystem.Infrastructure.Logging;

namespace ElevatorSystem.Console;

class Program
{
    static void Main(string[] args)
    {
        // Load configuration
        var config = new ElevatorConfiguration
        {
            NumberOfElevators = 3,
            TotalFloors = 10,
            MinFloor = 0,
            ElevatorCapacity = 10,
            SchedulingStrategy = "OptimalPath",
            EnableLogging = true,
            EnableDisplayPanel = false,
            EnableMetrics = true
        };

        config.Validate();

        // Initialize logger
        var logger = new FileLogger();
        logger.LogInfo("Elevator System Starting...");

        // Create building
        var building = new Building(config.TotalFloors, config.MinFloor);

        // Create elevators using factory
        IElevatorFactory factory = new DefaultElevatorFactory();
        var elevators = new List<Elevator>();

        for (int i = 1; i <= config.NumberOfElevators; i++)
        {
            var elevator = factory.CreateElevator(i, config.TotalFloors, config.ElevatorCapacity);
            elevators.Add(elevator);
            logger.LogInfo($"Created Elevator {i}");
        }

        // Create scheduling strategy
        ISchedulingStrategy strategy = config.SchedulingStrategy.ToLower() switch
        {
            "nearest" => new NearestElevatorStrategy(),
            "leastloaded" => new LeastLoadedStrategy(),
            "optimalpath" => new OptimalPathStrategy(),
            _ => new OptimalPathStrategy()
        };

        // Create elevator controller
        var controller = new ElevatorController(elevators, building, strategy);

        // Add observers
        if (config.EnableLogging)
        {
            var loggingObserver = new LoggingObserver();
            controller.AddObserverToAll(loggingObserver);
        }

        MetricsObserver? metricsObserver = null;
        if (config.EnableMetrics)
        {
            metricsObserver = new MetricsObserver();
            controller.AddObserverToAll(metricsObserver);
        }

        if (config.EnableDisplayPanel)
        {
            var displayObserver = new DisplayPanelObserver();
            controller.AddObserverToAll(displayObserver);
        }

        // Start the system
        controller.Start();

        // Run simulation or interactive mode
        if (args.Length > 0 && args[0] == "--demo")
        {
            RunDemoSimulation(controller, logger);
        }
        else
        {
            RunInteractiveMode(controller, logger);
        }

        // Stop the system
        controller.Stop();

        // Print metrics
        metricsObserver?.PrintMetrics();

        logger.LogInfo("Elevator System Stopped");
        System.Console.WriteLine("\nPress any key to exit...");
        System.Console.ReadKey();
    }

    static void RunDemoSimulation(ElevatorController controller, FileLogger logger)
    {
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine("\n╔═══════════════════════════════════════╗");
        System.Console.WriteLine("║      DEMO SIMULATION MODE             ║");
        System.Console.WriteLine("╚═══════════════════════════════════════╝\n");
        System.Console.ResetColor();

        logger.LogInfo("Starting demo simulation");

        // Simulate various scenarios
        System.Console.WriteLine("Scenario 1: Multiple external requests");
        controller.RequestElevator(5, Direction.Up);
        Thread.Sleep(1000);
        controller.RequestElevator(2, Direction.Down);
        Thread.Sleep(1000);
        controller.RequestElevator(8, Direction.Down);
        Thread.Sleep(3000);

        System.Console.WriteLine("\nScenario 2: Internal requests");
        controller.RequestFloor(1, 7);
        Thread.Sleep(2000);
        controller.RequestFloor(2, 3);
        Thread.Sleep(5000);

        System.Console.WriteLine("\nScenario 3: Rush hour simulation");
        for (int i = 1; i <= 5; i++)
        {
            controller.RequestElevator(i, Direction.Up);
            Thread.Sleep(500);
        }
        Thread.Sleep(10000);

        controller.PrintStatus();
        logger.LogInfo("Demo simulation completed");
    }

    static void RunInteractiveMode(ElevatorController controller, FileLogger logger)
    {
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine("\n╔═══════════════════════════════════════╗");
        System.Console.WriteLine("║      INTERACTIVE MODE                 ║");
        System.Console.WriteLine("╚═══════════════════════════════════════╝\n");
        System.Console.ResetColor();

        logger.LogInfo("Starting interactive mode");

        bool running = true;
        while (running)
        {
            DisplayMenu();
            var choice = System.Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        HandleExternalRequest(controller);
                        break;
                    case "2":
                        HandleInternalRequest(controller);
                        break;
                    case "3":
                        controller.PrintStatus();
                        break;
                    case "4":
                        RunQuickDemo(controller);
                        break;
                    case "5":
                        running = false;
                        break;
                    default:
                        System.Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"Error: {ex.Message}");
                System.Console.ResetColor();
                logger.LogError("Error in interactive mode", ex);
            }

            Thread.Sleep(500);
        }
    }

    static void DisplayMenu()
    {
        System.Console.WriteLine("\n┌──────────────────────────────────────┐");
        System.Console.WriteLine("│           MAIN MENU                  │");
        System.Console.WriteLine("├──────────────────────────────────────┤");
        System.Console.WriteLine("│ 1. External Request (call elevator)  │");
        System.Console.WriteLine("│ 2. Internal Request (select floor)   │");
        System.Console.WriteLine("│ 3. View System Status                │");
        System.Console.WriteLine("│ 4. Run Quick Demo                    │");
        System.Console.WriteLine("│ 5. Exit                              │");
        System.Console.WriteLine("└──────────────────────────────────────┘");
        System.Console.Write("\nEnter choice: ");
    }

    static void HandleExternalRequest(ElevatorController controller)
    {
        System.Console.Write("Enter floor number: ");
        if (int.TryParse(System.Console.ReadLine(), out int floor))
        {
            System.Console.Write("Direction (1=Up, 2=Down): ");
            if (int.TryParse(System.Console.ReadLine(), out int dirChoice))
            {
                var direction = dirChoice == 1 ? Direction.Up : Direction.Down;
                controller.RequestElevator(floor, direction);
            }
        }
    }

    static void HandleInternalRequest(ElevatorController controller)
    {
        System.Console.Write("Enter elevator ID: ");
        if (int.TryParse(System.Console.ReadLine(), out int elevatorId))
        {
            System.Console.Write("Enter destination floor: ");
            if (int.TryParse(System.Console.ReadLine(), out int floor))
            {
                controller.RequestFloor(elevatorId, floor);
            }
        }
    }

    static void RunQuickDemo(ElevatorController controller)
    {
        System.Console.WriteLine("\nRunning quick demo...");
        controller.RequestElevator(3, Direction.Up);
        Thread.Sleep(1000);
        controller.RequestElevator(7, Direction.Down);
        Thread.Sleep(1000);
        controller.RequestFloor(1, 5);
        Thread.Sleep(3000);
        controller.PrintStatus();
    }
}
