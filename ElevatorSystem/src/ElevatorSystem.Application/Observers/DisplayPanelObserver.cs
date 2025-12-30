using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Interfaces;

namespace ElevatorSystem.Application.Observers;

/// <summary>
/// Observer that displays elevator status on a virtual panel
/// </summary>
public class DisplayPanelObserver : IElevatorObserver
{
    public string ObserverName => "Display Panel Observer";

    public void Update(Elevator elevator)
    {
        // Simulate display panel update
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"┌─────────────────────────────────────┐");
        Console.WriteLine($"│ ELEVATOR {elevator.Id,-2} STATUS              │");
        Console.WriteLine($"├─────────────────────────────────────┤");
        Console.WriteLine($"│ Current Floor: {elevator.CurrentFloor,-20}│");
        Console.WriteLine($"│ Direction:     {elevator.Direction,-20}│");
        Console.WriteLine($"│ Door Status:   {elevator.DoorStatus,-20}│");
        Console.WriteLine($"│ State:         {elevator.CurrentStateName,-20}│");
        Console.WriteLine($"│ Pending:       {elevator.GetPendingRequestCount(),-20}│");
        Console.WriteLine($"└─────────────────────────────────────┘");
        Console.ResetColor();
    }
}
