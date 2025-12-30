namespace RideSharing.Domain.Exceptions;

/// <summary>
/// Exception thrown when an invalid trip state transition is attempted
/// </summary>
public class InvalidTripStateException : Exception
{
    public string CurrentState { get; }
    public string AttemptedState { get; }

    public InvalidTripStateException(string currentState, string attemptedState)
        : base($"Cannot transition from {currentState} to {attemptedState}")
    {
        CurrentState = currentState;
        AttemptedState = attemptedState;
    }

    public InvalidTripStateException(string message) : base(message)
    {
        CurrentState = string.Empty;
        AttemptedState = string.Empty;
    }
}
