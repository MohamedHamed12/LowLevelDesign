using System.Net.Http.Headers;
using IDM.Core.Enums;
using IDM.Core.Exceptions;
using IDM.Core.Interfaces;
using IDM.Core.Models;
using IDM.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IDM.Infrastructure.Services;

public class DownloadEngine : IDownloadEngine
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISegmentDownloader _segmentDownloader;
    private readonly IStorageManager _storage;
    private readonly IProgressTracker _progressTracker;
    private readonly DownloadConfiguration _config;
    private readonly ILogger<DownloadEngine> _logger;
    private readonly Dictionary<Guid, CancellationTokenSource> _downloadCancellations = new();

    public DownloadEngine(
        IHttpClientFactory httpClientFactory,
        ISegmentDownloader segmentDownloader,
        IStorageManager storage,
        IProgressTracker progressTracker,
        IOptions<DownloadConfiguration> config,
        ILogger<DownloadEngine> logger)
    {
        _httpClientFactory = httpClientFactory;
        _segmentDownloader = segmentDownloader;
        _storage = storage;
        _progressTracker = progressTracker;
        _config = config.Value;
        _logger = logger;
    }

    public async Task ExecuteDownloadAsync(DownloadTask task, CancellationToken cancellationToken = default)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _downloadCancellations[task.Id] = cts;

        try
        {
            task.OnStatusChanged(DownloadStatus.Initializing);
            
            var client = _httpClientFactory.CreateClient();
            var totalSize = await GetFileSizeAsync(client, task.Url, cts.Token);
            task.TotalBytes = totalSize;

            var supportsRange = await SupportsRangeRequestsAsync(client, task.Url, cts.Token);
            
            if (supportsRange && totalSize > _config.MinFileSizeForSegmentation)
            {
                await DownloadWithSegmentsAsync(task, totalSize, cts.Token);
            }
            else
            {
                await DownloadSingleStreamAsync(task, cts.Token);
            }

            task.OnStatusChanged(DownloadStatus.Completed);
            _logger.LogInformation("Download completed: {Id}", task.Id);
        }
        catch (OperationCanceledException)
        {
            task.OnStatusChanged(DownloadStatus.Cancelled);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Download failed: {Id}", task.Id);
            task.OnStatusChanged(DownloadStatus.Failed, ex.Message);
            throw new DownloadException(task.Id, "Download failed", ex);
        }
        finally
        {
            _downloadCancellations.Remove(task.Id);
            cts.Dispose();
        }
    }

    public Task PauseDownloadAsync(DownloadTask task, CancellationToken cancellationToken = default)
    {
        if (_downloadCancellations.TryGetValue(task.Id, out var cts))
        {
            cts.Cancel();
            task.OnStatusChanged(DownloadStatus.Paused);
        }
        return Task.CompletedTask;
    }

    public async Task ResumeDownloadAsync(DownloadTask task, CancellationToken cancellationToken = default)
    {
        task.OnStatusChanged(DownloadStatus.Pending);
        await ExecuteDownloadAsync(task, cancellationToken);
    }

    private async Task<long> GetFileSizeAsync(HttpClient client, string url, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Head, url);
        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return response.Content.Headers.ContentLength ?? 0;
    }

    private async Task<bool> SupportsRangeRequestsAsync(HttpClient client, string url, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Head, url);
        using var response = await client.SendAsync(request, cancellationToken);
        return response.Headers.AcceptRanges.Contains("bytes");
    }

    private async Task DownloadWithSegmentsAsync(DownloadTask task, long totalSize, CancellationToken cancellationToken)
    {
        task.OnStatusChanged(DownloadStatus.Downloading);

        if (task.Segments.Count == 0)
        {
            task.Segments = CreateSegments(task.Id, totalSize, _config.MaxSegmentsPerDownload);
        }

        var progressTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(_config.ProgressUpdateIntervalMs));
        var progressTask = Task.Run(async () =>
        {
            while (await progressTimer.WaitForNextTickAsync(cancellationToken))
            {
                _progressTracker.UpdateProgress(task);
            }
        }, cancellationToken);

        try
        {
            var downloadTasks = task.Segments
                .Where(s => s.Status != SegmentStatus.Completed)
                .Select(segment => _segmentDownloader.DownloadSegmentAsync(segment, task, cancellationToken));

            await Task.WhenAll(downloadTasks);

            task.OnStatusChanged(DownloadStatus.Merging);
            await _storage.MergeSegmentsAsync(task.Segments, task.Destination, cancellationToken);
            await _storage.CleanupTempFilesAsync(task.Id, cancellationToken);
        }
        finally
        {
            progressTimer.Dispose();
        }
    }

    private async Task DownloadSingleStreamAsync(DownloadTask task, CancellationToken cancellationToken)
    {
        task.OnStatusChanged(DownloadStatus.Downloading);

        var client = _httpClientFactory.CreateClient();
        using var response = await client.GetAsync(task.Url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var directory = Path.GetDirectoryName(task.Destination);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var fileStream = new FileStream(task.Destination, FileMode.Create, FileAccess.Write, FileShare.None, _config.BufferSize, true);

        var buffer = new byte[_config.BufferSize];
        long totalBytesRead = 0;
        int bytesRead;

        while ((bytesRead = await contentStream.ReadAsync(buffer, cancellationToken)) > 0)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
            totalBytesRead += bytesRead;
            task.DownloadedBytes = totalBytesRead;
            _progressTracker.UpdateProgress(task);
        }
    }

    private List<DownloadSegment> CreateSegments(Guid taskId, long totalSize, int segmentCount)
    {
        var segments = new List<DownloadSegment>();
        var segmentSize = totalSize / segmentCount;

        for (int i = 0; i < segmentCount; i++)
        {
            var startByte = i * segmentSize;
            var endByte = (i == segmentCount - 1) ? totalSize - 1 : (startByte + segmentSize - 1);

            segments.Add(new DownloadSegment
            {
                SegmentId = i,
                StartByte = startByte,
                EndByte = endByte,
                TempFilePath = _storage.CreateTempFilePath(taskId, i),
                Status = SegmentStatus.Pending
            });
        }

        return segments;
    }
}
