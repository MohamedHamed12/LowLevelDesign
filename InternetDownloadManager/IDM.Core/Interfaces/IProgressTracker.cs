using IDM.Core.Models;

namespace IDM.Core.Interfaces;

public interface IProgressTracker
{
    void UpdateProgress(DownloadTask task);
    double CalculateSpeed(DownloadTask task);
    TimeSpan CalculateETA(DownloadTask task);
    void RecordSpeedSample(Guid taskId, long bytes, DateTime timestamp);
}
