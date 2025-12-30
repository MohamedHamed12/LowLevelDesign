using RideSharing.Domain.Enums;

namespace RideSharing.Domain.Entities;

/// <summary>
/// Represents a notification sent to a user
/// </summary>
public class Notification
{
    public Guid Id { get; private set; }
    public User Recipient { get; private set; }
    public NotificationType Type { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; }

    public Notification(User recipient, NotificationType type, string title, string message)
    {
        Id = Guid.NewGuid();
        Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
        Type = type;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Message = message ?? throw new ArgumentNullException(nameof(message));
        CreatedAt = DateTime.UtcNow;
        IsRead = false;
        Metadata = new Dictionary<string, string>();
    }

    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }
    }

    public void AddMetadata(string key, string value)
    {
        Metadata[key] = value;
    }

    public string? GetMetadata(string key)
    {
        return Metadata.TryGetValue(key, out var value) ? value : null;
    }

    public override string ToString()
    {
        var status = IsRead ? "Read" : "Unread";
        return $"[{Type}] {Title} - {status}";
    }
}
