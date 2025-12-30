using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;

namespace RideSharing.Domain.Interfaces;

/// <summary>
/// Interface for sending notifications
/// </summary>
public interface INotificationService
{
    Task SendNotificationAsync(
        User user,
        NotificationType type,
        string title,
        string message,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default);

    Task SendBulkNotificationAsync(
        IEnumerable<User> users,
        NotificationType type,
        string title,
        string message,
        CancellationToken cancellationToken = default);
}
