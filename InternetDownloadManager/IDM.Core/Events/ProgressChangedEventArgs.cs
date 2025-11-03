namespace IDM.Core.Events;

public class ProgressChangedEventArgs : EventArgs
{
    public Guid TaskId { get; }
    public double Progress { get; }
    public long DownloadedBytes { get; }
    public long TotalBytes { get; }
    public double Speed { get; }
    public TimeSpan EstimatedTimeRemaining { get; }

    public ProgressChangedEventArgs(Guid taskId, double progress, long downloadedBytes, long totalBytes, double speed, TimeSpan eta)
    {
        TaskId = taskId;
        Progress = progress;
        DownloadedBytes = downloadedBytes;
        TotalBytes = totalBytes;
        Speed = speed;
        EstimatedTimeRemaining = eta;
    }
}
