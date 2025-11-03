using IDM.Core.Models;

namespace IDM.Core.Interfaces;

public interface IDownloadEngine
{
    Task ExecuteDownloadAsync(DownloadTask task, CancellationToken cancellationToken = default);
    Task PauseDownloadAsync(DownloadTask task, CancellationToken cancellationToken = default);
    Task ResumeDownloadAsync(DownloadTask task, CancellationToken cancellationToken = default);
}
