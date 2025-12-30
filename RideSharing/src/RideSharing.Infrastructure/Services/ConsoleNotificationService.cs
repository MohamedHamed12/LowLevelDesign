using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;
using RideSharing.Domain.Interfaces;

namespace RideSharing.Infrastructure.Services;

/// <summary>
/// Console-based notification service
/// In production, this would integrate with SMS, email, push notification services
/// </summary>
public class ConsoleNotificationService : INotificationService
{
    private readonly object _consoleLock = new();

    public Task SendNotificationAsync(
        User user,
        NotificationType type,
        string title,
        string message,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        lock (_consoleLock)
        {
            var color = type switch
            {
                NotificationType.TripUpdate => ConsoleColor.Cyan,
                NotificationType.PaymentUpdate => ConsoleColor.Green,
                _ => ConsoleColor.White
            };

            Console.ForegroundColor = color;
            Console.WriteLine($"\n📱 [{type}] {user.Name}: {title}");
            Console.WriteLine($"   {message}");
            Console.ResetColor();
        }

        return Task.CompletedTask;
    }

    public Task SendBulkNotificationAsync(
        IEnumerable<User> users,
        NotificationType type,
        string title,
        string message,
        CancellationToken cancellationToken = default)
    {
        foreach (var user in users)
        {
            SendNotificationAsync(user, type, title, message, null, cancellationToken);
        }

        return Task.CompletedTask;
    }
}
