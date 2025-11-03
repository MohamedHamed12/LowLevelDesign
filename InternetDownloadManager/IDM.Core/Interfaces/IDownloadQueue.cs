using IDM.Core.Models;

namespace IDM.Core.Interfaces;

public interface IDownloadQueue
{
    void Enqueue(DownloadTask task);
    Task<DownloadTask?> DequeueAsync(CancellationToken cancellationToken = default);
    bool TryRemove(Guid id, out DownloadTask? task);
    IEnumerable<DownloadTask> GetPendingTasks();
    int Count { get; }
}
