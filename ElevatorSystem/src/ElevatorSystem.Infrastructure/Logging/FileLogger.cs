namespace ElevatorSystem.Infrastructure.Logging;

/// <summary>
/// Simple file logger for elevator system events
/// </summary>
public class FileLogger
{
    private readonly string _logFilePath;
    private readonly object _lock = new();

    public FileLogger(string logDirectory = "logs")
    {
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        _logFilePath = Path.Combine(logDirectory, $"elevator_{timestamp}.log");
    }

    public void Log(string message)
    {
        lock (_lock)
        {
            try
            {
                var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
                File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write to log: {ex.Message}");
            }
        }
    }

    public void LogInfo(string message)
    {
        Log($"[INFO] {message}");
    }

    public void LogWarning(string message)
    {
        Log($"[WARN] {message}");
    }

    public void LogError(string message)
    {
        Log($"[ERROR] {message}");
    }

    public void LogError(string message, Exception exception)
    {
        Log($"[ERROR] {message} - Exception: {exception.Message}");
        Log($"[ERROR] StackTrace: {exception.StackTrace}");
    }
}
