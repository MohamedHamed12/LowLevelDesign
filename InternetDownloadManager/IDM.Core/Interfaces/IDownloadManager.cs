using IDM.Core.Models;

namespace IDM.Core.Interfaces;

public interface IDownloadManager
{
    Task<DownloadTask> AddDownloadAsync(string url, string destination, CancellationToken cancellationToken = default);
    Task PauseDownloadAsync(Guid id, CancellationToken cancellationToken = default);
    Task ResumeDownloadAsync(Guid id, CancellationToken cancellationToken = default);
    Task CancelDownloadAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DownloadTask?> GetDownloadAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DownloadTask>> GetAllDownloadsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<DownloadTask>> GetActiveDownloadsAsync(CancellationToken cancellationToken = default);
}
