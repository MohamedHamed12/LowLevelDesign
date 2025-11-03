using IDM.Core.Models;

namespace IDM.Core.Interfaces;

public interface IStorageManager
{
    Task SaveMetadataAsync(DownloadTask task, CancellationToken cancellationToken = default);
    Task<DownloadTask?> LoadMetadataAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteMetadataAsync(Guid id, CancellationToken cancellationToken = default);
    string CreateTempFilePath(Guid taskId, int segmentId);
    Task MergeSegmentsAsync(List<DownloadSegment> segments, string destination, CancellationToken cancellationToken = default);
    Task CleanupTempFilesAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DownloadTask>> LoadAllMetadataAsync(CancellationToken cancellationToken = default);
}
