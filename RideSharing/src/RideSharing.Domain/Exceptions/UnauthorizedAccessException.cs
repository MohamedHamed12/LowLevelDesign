namespace RideSharing.Domain.Exceptions;

/// <summary>
/// Exception thrown when a user is not authorized to perform an action
/// </summary>
public class UnauthorizedAccessException : Exception
{
    public Guid UserId { get; }
    public string Action { get; }

    public UnauthorizedAccessException(Guid userId, string action)
        : base($"User {userId} is not authorized to perform action: {action}")
    {
        UserId = userId;
        Action = action;
    }

    public UnauthorizedAccessException(string message) : base(message)
    {
        UserId = Guid.Empty;
        Action = string.Empty;
    }
}
