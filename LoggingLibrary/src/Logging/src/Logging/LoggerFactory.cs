using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Logging;
public sealed class LoggerFactory : IDisposable
{
    private readonly LoggingOptions _options;
    private readonly Channel<LogEvent> _channel;
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _dispatcherTask;
    private volatile LogLevel _minLevel = LogLevel.Trace;

    public LoggerFactory(LoggingOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        var chOptions = new BoundedChannelOptions(_options.ChannelCapacity)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.DropOldest
        };
        _channel = Channel.CreateBounded<LogEvent>(chOptions);
        _dispatcherTask = Task.Run(() => DispatcherAsync(_cts.Token));
    }

    public ILogger CreateLogger(string category) => new Logger(category, this);

    public void Enqueue(LogEvent evt)
    {
        // best-effort write
        _channel.Writer.TryWrite(evt);
    }

    public bool IsEnabled(LogLevel level) => level >= _minLevel;

    public void SetMinimumLevel(LogLevel level) => _minLevel = level;

    private async Task DispatcherAsync(CancellationToken ct)
    {
        var batch = new List<LogEvent>(_options.BatchSize);
        var reader = _channel.Reader;
        while (!ct.IsCancellationRequested)
        {
            try
            {
                // wait for first item
                LogEvent first;
                if (!await reader.WaitToReadAsync(ct).ConfigureAwait(false))
                    break;
                while (reader.TryRead(out first))
                {
                    batch.Add(first);
                    break;
                }

                var sw = System.Diagnostics.Stopwatch.StartNew();
                // collect upto BatchSize within BatchInterval
                while (batch.Count < _options.BatchSize && sw.Elapsed < _options.BatchInterval)
                {
                    if (reader.TryRead(out var next))
                    {
                        batch.Add(next);
                    }
                    else
                    {
                        // small pause
                        await Task.Delay(10, ct).ConfigureAwait(false);
                    }
                }

                if (batch.Count > 0)
                {
                    // send to sinks in parallel, but wait for them (careful: don't let one sink crash)
                    var tasks = new List<Task>();
                    foreach (var sink in _options.Sinks)
                    {
                        try
                        {
                            tasks.Add(sink.EmitBatchAsync(batch, ct));
                        }
                        catch
                        {
                            // ignore sink-add exceptions
                        }
                    }
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                    batch.Clear();
                }
            }
            catch (OperationCanceledException) { break; }
            catch (Exception)
            {
                // swallow and continue
            }
        }

        // flush remaining
        var remaining = new List<LogEvent>();
        while (reader.TryRead(out var ev)) remaining.Add(ev);
        if (remaining.Count > 0)
        {
            foreach (var sink in _options.Sinks)
            {
                try { await sink.EmitBatchAsync(remaining, CancellationToken.None).ConfigureAwait(false); }
                catch { }
            }
        }
    }

    public async Task StopAsync()
    {
        _cts.Cancel();
        _channel.Writer.Complete();
        try { await _dispatcherTask.ConfigureAwait(false); } catch { }
    }

    public void Dispose()
    {
        try { StopAsync().GetAwaiter().GetResult(); } catch { }
        _cts.Dispose();
    }
}
