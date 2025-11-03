using System.Collections.Concurrent;
using IDM.Core.Enums;
using IDM.Core.Interfaces;
using IDM.Core.Models;
using Microsoft.Extensions.Logging;

namespace IDM.Infrastructure.Services;

public class DownloadManager : IDownloadManager
{
    private readonly ConcurrentDictionary<Guid, DownloadTask> _downloads = new();
    private readonly IDownloadQueue _queue;
    private readonly IDownloadEngine _engine;
    private readonly IStorageManager _storage;
    private readonly ILogger<DownloadManager> _logger;
    private readonly SemaphoreSlim _concurrencySemaphore;
    private readonly CancellationTokenSource _shutdownCts = new();

    public DownloadManager(
        IDownloadQueue queue,
        IDownloadEngine engine,
        IStorageManager storage,
        ILogger<DownloadManager> logger,
        int maxConcurrentDownloads = 3)
    {
        _queue = queue;
        _engine = engine;
        _storage = storage;
        _logger = logger;
        _concurrencySemaphore = new SemaphoreSlim(maxConcurrentDownloads, maxConcurrentDownloads);

        _ = Task.Run(ProcessQueueAsync);
        _ = Task.Run(LoadExistingDownloadsAsync);
    }

    public async Task<DownloadTask> AddDownloadAsync(string url, string destination, CancellationToken cancellationToken = default)
    {
        var task = new DownloadTask
        {
            Url = url,
            Destination = destination
        };

        _downloads.TryAdd(task.Id, task);
        _queue.Enqueue(task);

        await _storage.SaveMetadataAsync(task, cancellationToken);
        
        _logger.LogInformation("Download added: {Id} - {Url}", task.Id, url);
        
        return task;
    }

    public async Task PauseDownloadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (_downloads.TryGetValue(id, out var task))
        {
            await _engine.PauseDownloadAsync(task, cancellationToken);
            await _storage.SaveMetadataAsync(task, cancellationToken);
            _logger.LogInformation("Download paused: {Id}", id);
        }
    }

    public async Task ResumeDownloadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (_downloads.TryGetValue(id, out var task) && task.Status == DownloadStatus.Paused)
        {
            _queue.Enqueue(task);
            await _storage.SaveMetadataAsync(task, cancellationToken);
            _logger.LogInformation("Download resumed: {Id}", id);
        }
    }

    public async Task CancelDownloadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (_downloads.TryGetValue(id, out var task))
        {
            task.OnStatusChanged(DownloadStatus.Cancelled);
            await _storage.CleanupTempFilesAsync(id, cancellationToken);
            await _storage.DeleteMetadataAsync(id, cancellationToken);
            _logger.LogInformation("Download cancelled: {Id}", id);
        }
    }

    public Task<DownloadTask?> GetDownloadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _downloads.TryGetValue(id, out var task);
        return Task.FromResult(task);
    }

    public Task<IEnumerable<DownloadTask>> GetAllDownloadsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<DownloadTask>>(_downloads.Values.ToList());
    }

    public Task<IEnumerable<DownloadTask>> GetActiveDownloadsAsync(CancellationToken cancellationToken = default)
    {
        var active = _downloads.Values
            .Where(t => t.Status == DownloadStatus.Downloading || t.Status == DownloadStatus.Pending)
            .ToList();
        return Task.FromResult<IEnumerable<DownloadTask>>(active);
    }

    private async Task ProcessQueueAsync()
    {
        while (!_shutdownCts.Token.IsCancellationRequested)
        {
            try
            {
                var task = await _queue.DequeueAsync(_shutdownCts.Token);
                if (task != null)
                {
                    await _concurrencySemaphore.WaitAsync(_shutdownCts.Token);
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _engine.ExecuteDownloadAsync(task, _shutdownCts.Token);
                            await _storage.SaveMetadataAsync(task, _shutdownCts.Token);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error downloading task {Id}", task.Id);
                            task.OnStatusChanged(DownloadStatus.Failed, ex.Message);
                        }
                        finally
                        {
                            _concurrencySemaphore.Release();
                        }
                    }, _shutdownCts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in queue processing");
                await Task.Delay(1000, _shutdownCts.Token);
            }
        }
    }

    private async Task LoadExistingDownloadsAsync()
    {
        try
        {
            var tasks = await _storage.LoadAllMetadataAsync();
            foreach (var task in tasks)
            {
                if (task.Status == DownloadStatus.Downloading || task.Status == DownloadStatus.Pending)
                {
                    task.OnStatusChanged(DownloadStatus.Paused);
                }
                _downloads.TryAdd(task.Id, task);
            }
            _logger.LogInformation("Loaded {Count} existing downloads", _downloads.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading existing downloads");
        }
    }
}
