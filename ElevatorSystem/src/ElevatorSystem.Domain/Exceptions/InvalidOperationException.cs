namespace ElevatorSystem.Domain.Exceptions;

/// <summary>
/// Exception thrown when an invalid elevator operation is attempted
/// </summary>
public class InvalidOperationException : Exception
{
    public InvalidOperationException(string message) : base(message)
    {
    }

    public InvalidOperationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
