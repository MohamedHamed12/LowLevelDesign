using System.Net.Http.Headers;
using IDM.Core.Enums;
using IDM.Core.Interfaces;
using IDM.Core.Models;
using IDM.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IDM.Infrastructure.Services;

public class SegmentDownloader : ISegmentDownloader
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IProgressTracker _progressTracker;
    private readonly DownloadConfiguration _config;
    private readonly ILogger<SegmentDownloader> _logger;

    public SegmentDownloader(
        IHttpClientFactory httpClientFactory,
        IProgressTracker progressTracker,
        IOptions<DownloadConfiguration> config,
        ILogger<SegmentDownloader> logger)
    {
        _httpClientFactory = httpClientFactory;
        _progressTracker = progressTracker;
        _config = config.Value;
        _logger = logger;
    }

    public async Task DownloadSegmentAsync(DownloadSegment segment, DownloadTask task, CancellationToken cancellationToken = default)
    {
        var retryCount = 0;
        
        while (retryCount < _config.MaxRetries)
        {
            try
            {
                segment.Status = SegmentStatus.Downloading;
                
                var client = _httpClientFactory.CreateClient();
                using var request = new HttpRequestMessage(HttpMethod.Get, task.Url);
                
                var startByte = segment.StartByte + segment.DownloadedBytes;
                request.Headers.Range = new RangeHeaderValue(startByte, segment.EndByte);

                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                response.EnsureSuccessStatusCode();

                var directory = Path.GetDirectoryName(segment.TempFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var fileStream = new FileStream(
                    segment.TempFilePath, 
                    segment.DownloadedBytes > 0 ? FileMode.Append : FileMode.Create, 
                    FileAccess.Write, 
                    FileShare.None, 
                    _config.BufferSize, 
                    true);

                var buffer = new byte[_config.BufferSize];
                int bytesRead;

                while ((bytesRead = await contentStream.ReadAsync(buffer, cancellationToken)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                    segment.DownloadedBytes += bytesRead;
                    _progressTracker.RecordSpeedSample(task.Id, bytesRead, DateTime.UtcNow);
                }

                segment.Status = SegmentStatus.Completed;
                _logger.LogDebug("Segment {SegmentId} completed for task {TaskId}", segment.SegmentId, task.Id);
                return;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                retryCount++;
                segment.ErrorMessage = ex.Message;
                
                if (retryCount < _config.MaxRetries)
                {
                    _logger.LogWarning(ex, "Segment {SegmentId} failed, retry {RetryCount}/{MaxRetries}", 
                        segment.SegmentId, retryCount, _config.MaxRetries);
                    await Task.Delay(TimeSpan.FromSeconds(_config.RetryDelaySeconds), cancellationToken);
                }
                else
                {
                    segment.Status = SegmentStatus.Failed;
                    _logger.LogError(ex, "Segment {SegmentId} failed after {RetryCount} retries", 
                        segment.SegmentId, _config.MaxRetries);
                    throw;
                }
            }
        }
    }
}
