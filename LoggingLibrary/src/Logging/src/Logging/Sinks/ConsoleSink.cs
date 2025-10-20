using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Logging;
public sealed class ConsoleSink : ILogSink
{
    public Task EmitBatchAsync(IEnumerable<LogEvent> events, CancellationToken ct)
    {
        foreach (var e in events)
        {
            var line = Format(e);
            // write without blocking the caller
            try { Console.WriteLine(line); } catch { }
        }
        return Task.CompletedTask;
    }

    private static string Format(LogEvent e)
    {
        var baseMsg = $"{e.Timestamp:O} [{e.Level}] {e.Category}: {e.Message}";
        if (e.Properties != null && e.Properties.Count > 0)
        {
            baseMsg += " | " + string.Join(", ", e.Properties);
        }
        if (e.Exception != null)
            baseMsg += $" => {e.Exception.GetType().Name}: {e.Exception.Message}";
        return baseMsg;
    }
}
