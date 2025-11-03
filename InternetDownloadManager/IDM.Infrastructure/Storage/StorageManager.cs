using System.Text.Json;
using IDM.Core.Interfaces;
using IDM.Core.Models;
using IDM.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IDM.Infrastructure.Storage;

public class StorageManager : IStorageManager
{
    private readonly DownloadConfiguration _config;
    private readonly ILogger<StorageManager> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public StorageManager(IOptions<DownloadConfiguration> config, ILogger<StorageManager> logger)
    {
        _config = config.Value;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions { WriteIndented = true };

        EnsureDirectoriesExist();
    }

    public async Task SaveMetadataAsync(DownloadTask task, CancellationToken cancellationToken = default)
    {
        try
        {
            var filePath = GetMetadataFilePath(task.Id);
            var json = JsonSerializer.Serialize(task, _jsonOptions);
            await File.WriteAllTextAsync(filePath, json, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save metadata for task {TaskId}", task.Id);
        }
    }

    public async Task<DownloadTask?> LoadMetadataAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var filePath = GetMetadataFilePath(id);
            if (!File.Exists(filePath))
                return null;

            var json = await File.ReadAllTextAsync(filePath, cancellationToken);
            return JsonSerializer.Deserialize<DownloadTask>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load metadata for task {TaskId}", id);
            return null;
        }
    }

    public async Task<IEnumerable<DownloadTask>> LoadAllMetadataAsync(CancellationToken cancellationToken = default)
    {
        var tasks = new List<DownloadTask>();

        try
        {
            var metadataFiles = Directory.GetFiles(_config.MetadataDirectory, "*.json");
            
            foreach (var file in metadataFiles)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(file, cancellationToken);
                    var task = JsonSerializer.Deserialize<DownloadTask>(json);
                    if (task != null)
                        tasks.Add(task);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to load metadata from {File}", file);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load all metadata");
        }

        return tasks;
    }

    public Task DeleteMetadataAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var filePath = GetMetadataFilePath(id);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete metadata for task {TaskId}", id);
        }

        return Task.CompletedTask;
    }

    public string CreateTempFilePath(Guid taskId, int segmentId)
    {
        var taskDirectory = Path.Combine(_config.TempDirectory, taskId.ToString());
        if (!Directory.Exists(taskDirectory))
            Directory.CreateDirectory(taskDirectory);

        return Path.Combine(taskDirectory, $"segment_{segmentId}.tmp");
    }

    public async Task MergeSegmentsAsync(List<DownloadSegment> segments, string destination, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(destination);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var outputStream = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None);

        var orderedSegments = segments.OrderBy(s => s.SegmentId).ToList();

        foreach (var segment in orderedSegments)
        {
            if (!File.Exists(segment.TempFilePath))
            {
                throw new FileNotFoundException($"Segment file not found: {segment.TempFilePath}");
            }

            using var inputStream = new FileStream(segment.TempFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            await inputStream.CopyToAsync(outputStream, cancellationToken);
        }

        _logger.LogInformation("Merged {Count} segments to {Destination}", segments.Count, destination);
    }

    public Task CleanupTempFilesAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        try
        {
            var taskDirectory = Path.Combine(_config.TempDirectory, taskId.ToString());
            if (Directory.Exists(taskDirectory))
            {
                Directory.Delete(taskDirectory, true);
                _logger.LogDebug("Cleaned up temp files for task {TaskId}", taskId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup temp files for task {TaskId}", taskId);
        }

        return Task.CompletedTask;
    }

    private string GetMetadataFilePath(Guid id)
    {
        return Path.Combine(_config.MetadataDirectory, $"{id}.json");
    }

    private void EnsureDirectoriesExist()
    {
        if (!Directory.Exists(_config.TempDirectory))
            Directory.CreateDirectory(_config.TempDirectory);

        if (!Directory.Exists(_config.MetadataDirectory))
            Directory.CreateDirectory(_config.MetadataDirectory);
    }
}
