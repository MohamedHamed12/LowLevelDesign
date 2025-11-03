using IDM.Core.Models;

namespace IDM.Core.Interfaces;

public interface ISegmentDownloader
{
    Task DownloadSegmentAsync(DownloadSegment segment, DownloadTask task, CancellationToken cancellationToken = default);
}
