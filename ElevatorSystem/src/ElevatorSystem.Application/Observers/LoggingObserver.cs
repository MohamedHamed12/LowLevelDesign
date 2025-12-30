using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Interfaces;

namespace ElevatorSystem.Application.Observers;

/// <summary>
/// Observer that logs elevator state changes
/// </summary>
public class LoggingObserver : IElevatorObserver
{
    public string ObserverName => "Logging Observer";

    public void Update(Elevator elevator)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        var message = $"[{timestamp}] {elevator}";
        
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}
