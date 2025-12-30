using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Interfaces;

namespace ElevatorSystem.Application.Observers;

/// <summary>
/// Observer that collects metrics about elevator performance
/// </summary>
public class MetricsObserver : IElevatorObserver
{
    private readonly Dictionary<int, List<DateTime>> _floorVisits = new();
    private int _totalMovements = 0;
    private int _totalDoorOperations = 0;

    public string ObserverName => "Metrics Observer";

    public void Update(Elevator elevator)
    {
        // Track floor visits
        if (!_floorVisits.ContainsKey(elevator.CurrentFloor))
        {
            _floorVisits[elevator.CurrentFloor] = new List<DateTime>();
        }
        _floorVisits[elevator.CurrentFloor].Add(DateTime.Now);

        // Increment counters
        _totalMovements++;
        if (elevator.DoorStatus == Domain.Enums.DoorStatus.Open)
        {
            _totalDoorOperations++;
        }
    }

    public void PrintMetrics()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n════════════════════════════════════");
        Console.WriteLine("         ELEVATOR METRICS");
        Console.WriteLine("════════════════════════════════════");
        Console.WriteLine($"Total Movements:      {_totalMovements}");
        Console.WriteLine($"Door Operations:      {_totalDoorOperations}");
        Console.WriteLine($"Unique Floors Visited: {_floorVisits.Count}");
        Console.WriteLine("════════════════════════════════════\n");
        Console.ResetColor();
    }
}
