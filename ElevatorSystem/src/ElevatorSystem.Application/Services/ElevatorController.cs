using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Enums;
using ElevatorSystem.Domain.Interfaces;
using System.Collections.Concurrent;

namespace ElevatorSystem.Application.Services;

/// <summary>
/// Main controller for managing multiple elevators
/// Implements Dependency Inversion Principle by depending on abstractions
/// </summary>
public class ElevatorController
{
    private readonly List<Elevator> _elevators;
    private readonly Building _building;
    private readonly ISchedulingStrategy _scheduler;
    private readonly ConcurrentQueue<Request> _pendingRequests;
    private readonly object _lock = new();
    private bool _isRunning;

    public IReadOnlyList<Elevator> Elevators => _elevators.AsReadOnly();
    public Building Building => _building;

    public ElevatorController(
        List<Elevator> elevators,
        Building building,
        ISchedulingStrategy scheduler)
    {
        _elevators = elevators ?? throw new ArgumentNullException(nameof(elevators));
        _building = building ?? throw new ArgumentNullException(nameof(building));
        _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
        _pendingRequests = new ConcurrentQueue<Request>();
        _isRunning = false;
    }

    /// <summary>
    /// Requests an elevator for external call (from floor)
    /// </summary>
    public void RequestElevator(int floor, Direction direction)
    {
        _building.ValidateFloor(floor);

        if (direction == Direction.None)
            throw new ArgumentException("Direction must be Up or Down for external requests", nameof(direction));

        var request = new Request(floor, direction, RequestType.External);
        _pendingRequests.Enqueue(request);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[REQUEST] External call from floor {floor} going {direction}");
        Console.ResetColor();
    }

    /// <summary>
    /// Requests destination floor from inside elevator
    /// </summary>
    public void RequestFloor(int elevatorId, int destinationFloor)
    {
        _building.ValidateFloor(destinationFloor);

        var elevator = _elevators.FirstOrDefault(e => e.Id == elevatorId);
        if (elevator == null)
            throw new ArgumentException($"Elevator {elevatorId} not found", nameof(elevatorId));

        elevator.AddRequest(destinationFloor);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[REQUEST] Internal call in elevator {elevatorId} to floor {destinationFloor}");
        Console.ResetColor();
    }

    /// <summary>
    /// Processes all pending requests
    /// </summary>
    public void ProcessRequests()
    {
        while (_pendingRequests.TryDequeue(out var request))
        {
            var selectedElevator = _scheduler.SelectElevator(_elevators, request);

            if (selectedElevator != null)
            {
                selectedElevator.AddRequest(request.SourceFloor);
                
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"[ASSIGNED] Elevator {selectedElevator.Id} assigned to floor {request.SourceFloor}");
                Console.ResetColor();
            }
            else
            {
                // Re-queue if no elevator available
                _pendingRequests.Enqueue(request);
                Thread.Sleep(100); // Wait a bit before retrying
            }
        }
    }

    /// <summary>
    /// Starts the elevator system
    /// </summary>
    public void Start()
    {
        lock (_lock)
        {
            if (_isRunning)
                return;

            _isRunning = true;
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("═══════════════════════════════════════");
        Console.WriteLine("  ELEVATOR SYSTEM STARTED");
        Console.WriteLine($"  Strategy: {_scheduler.StrategyName}");
        Console.WriteLine($"  Elevators: {_elevators.Count}");
        Console.WriteLine($"  Floors: {_building.MinFloor} to {_building.MaxFloor}");
        Console.WriteLine("═══════════════════════════════════════");
        Console.ResetColor();

        // Start processing threads
        var requestProcessorThread = new Thread(() =>
        {
            while (_isRunning)
            {
                ProcessRequests();
                Thread.Sleep(500);
            }
        });
        requestProcessorThread.Start();

        // Start elevator processing threads
        foreach (var elevator in _elevators)
        {
            var elevatorThread = new Thread(() =>
            {
                while (_isRunning)
                {
                    elevator.ProcessState();
                    Thread.Sleep(1000); // Process every second
                }
            });
            elevatorThread.Start();
        }
    }

    /// <summary>
    /// Stops the elevator system
    /// </summary>
    public void Stop()
    {
        lock (_lock)
        {
            _isRunning = false;
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n═══════════════════════════════════════");
        Console.WriteLine("  ELEVATOR SYSTEM STOPPED");
        Console.WriteLine("═══════════════════════════════════════");
        Console.ResetColor();
    }

    /// <summary>
    /// Gets status of all elevators
    /// </summary>
    public void PrintStatus()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n╔═══════════════════════════════════════╗");
        Console.WriteLine("║       ELEVATOR SYSTEM STATUS          ║");
        Console.WriteLine("╠═══════════════════════════════════════╣");
        
        foreach (var elevator in _elevators)
        {
            Console.WriteLine($"║ {elevator.ToString().PadRight(37)} ║");
        }
        
        Console.WriteLine("╚═══════════════════════════════════════╝");
        Console.ResetColor();
    }

    /// <summary>
    /// Adds an observer to all elevators
    /// </summary>
    public void AddObserverToAll(IElevatorObserver observer)
    {
        foreach (var elevator in _elevators)
        {
            elevator.AddObserver(observer);
        }
    }
}
