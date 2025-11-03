using IDM.Core.Enums;

namespace IDM.Core.Events;

public class StatusChangedEventArgs : EventArgs
{
    public Guid TaskId { get; }
    public DownloadStatus OldStatus { get; }
    public DownloadStatus NewStatus { get; }
    public string? ErrorMessage { get; }

    public StatusChangedEventArgs(Guid taskId, DownloadStatus oldStatus, DownloadStatus newStatus, string? errorMessage = null)
    {
        TaskId = taskId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        ErrorMessage = errorMessage;
    }
}
