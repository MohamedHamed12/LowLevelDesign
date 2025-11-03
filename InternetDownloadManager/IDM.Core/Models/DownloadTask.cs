using IDM.Core.Enums;
using IDM.Core.Events;

namespace IDM.Core.Models;

public class DownloadTask
{
    public Guid Id { get; init; }
    public required string Url { get; init; }
    public required string Destination { get; init; }
    public DownloadStatus Status { get; set; }
    public long TotalBytes { get; set; }
    public long DownloadedBytes { get; set; }
    public double Progress => TotalBytes > 0 ? (double)DownloadedBytes / TotalBytes * 100 : 0;
    public double Speed { get; set; }
    public TimeSpan EstimatedTimeRemaining { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public List<DownloadSegment> Segments { get; set; } = new();
    
    public event EventHandler<ProgressChangedEventArgs>? ProgressChanged;
    public event EventHandler<StatusChangedEventArgs>? StatusChanged;

    public DownloadTask()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Status = DownloadStatus.Pending;
    }

    public void OnProgressChanged(long downloadedBytes, double speed, TimeSpan eta)
    {
        DownloadedBytes = downloadedBytes;
        Speed = speed;
        EstimatedTimeRemaining = eta;
        ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(Id, Progress, downloadedBytes, TotalBytes, speed, eta));
    }

    public void OnStatusChanged(DownloadStatus newStatus, string? errorMessage = null)
    {
        var oldStatus = Status;
        Status = newStatus;
        ErrorMessage = errorMessage;

        if (newStatus == DownloadStatus.Downloading && StartedAt == null)
            StartedAt = DateTime.UtcNow;
        
        if (newStatus == DownloadStatus.Completed)
            CompletedAt = DateTime.UtcNow;

        StatusChanged?.Invoke(this, new StatusChangedEventArgs(Id, oldStatus, newStatus, errorMessage));
    }
}
