using System.Collections.Concurrent;
using IDM.Core.Interfaces;
using IDM.Core.Models;

namespace IDM.Infrastructure.Services;

public class ProgressTracker : IProgressTracker
{
    private readonly ConcurrentDictionary<Guid, List<SpeedSample>> _speedSamples = new();
    private readonly TimeSpan _sampleWindow = TimeSpan.FromSeconds(5);

    public void UpdateProgress(DownloadTask task)
    {
        var downloadedBytes = task.Segments.Sum(s => s.DownloadedBytes);
        var speed = CalculateSpeed(task);
        var eta = CalculateETA(task);

        task.OnProgressChanged(downloadedBytes, speed, eta);
    }

    public double CalculateSpeed(DownloadTask task)
    {
        if (!_speedSamples.TryGetValue(task.Id, out var samples))
            return 0;

        var cutoffTime = DateTime.UtcNow - _sampleWindow;
        var recentSamples = samples.Where(s => s.Timestamp >= cutoffTime).ToList();

        if (recentSamples.Count < 2)
            return 0;

        var totalBytes = recentSamples.Sum(s => s.Bytes);
        var timeSpan = recentSamples.Max(s => s.Timestamp) - recentSamples.Min(s => s.Timestamp);

        return timeSpan.TotalSeconds > 0 ? totalBytes / timeSpan.TotalSeconds : 0;
    }

    public TimeSpan CalculateETA(DownloadTask task)
    {
        var speed = CalculateSpeed(task);
        if (speed <= 0)
            return TimeSpan.MaxValue;

        var remainingBytes = task.TotalBytes - task.DownloadedBytes;
        var seconds = remainingBytes / speed;

        return TimeSpan.FromSeconds(seconds);
    }

    public void RecordSpeedSample(Guid taskId, long bytes, DateTime timestamp)
    {
        var samples = _speedSamples.GetOrAdd(taskId, _ => new List<SpeedSample>());
        
        lock (samples)
        {
            samples.Add(new SpeedSample { Bytes = bytes, Timestamp = timestamp });
            
            var cutoffTime = timestamp - _sampleWindow;
            samples.RemoveAll(s => s.Timestamp < cutoffTime);
        }
    }

    private class SpeedSample
    {
        public long Bytes { get; init; }
        public DateTime Timestamp { get; init; }
    }
}
