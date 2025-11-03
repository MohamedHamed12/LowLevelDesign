using System.Collections.Concurrent;
using System.Threading.Channels;
using IDM.Core.Interfaces;
using IDM.Core.Models;

namespace IDM.Infrastructure.Services;

public class DownloadQueue : IDownloadQueue
{
    private readonly Channel<DownloadTask> _channel;
    private readonly ConcurrentDictionary<Guid, DownloadTask> _tasks = new();

    public int Count => _tasks.Count;

    public DownloadQueue()
    {
        _channel = Channel.CreateUnbounded<DownloadTask>(new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = false
        });
    }

    public void Enqueue(DownloadTask task)
    {
        if (_tasks.TryAdd(task.Id, task))
        {
            _channel.Writer.TryWrite(task);
        }
    }

    public async Task<DownloadTask?> DequeueAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var task = await _channel.Reader.ReadAsync(cancellationToken);
            _tasks.TryRemove(task.Id, out _);
            return task;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }

    public bool TryRemove(Guid id, out DownloadTask? task)
    {
        return _tasks.TryRemove(id, out task);
    }

    public IEnumerable<DownloadTask> GetPendingTasks()
    {
        return _tasks.Values.ToList();
    }
}
