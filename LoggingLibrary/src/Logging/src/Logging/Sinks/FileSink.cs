using Logging.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Logging;
public sealed class FileSink : ILogSink, IDisposable
{
    private readonly RollingFileWriter _writer;

    public FileSink(string directory, string fileName, long maxBytes = 10 * 1024 * 1024, int maxFiles = 5)
    {
        _writer = new RollingFileWriter(directory, fileName, maxBytes, maxFiles);
    }

    public async Task EmitBatchAsync(IEnumerable<LogEvent> events, CancellationToken ct)
    {
        // write sequentially to keep file order
        foreach (var e in events)
        {
            ct.ThrowIfCancellationRequested();
            var sb = new StringBuilder();
            sb.Append(e.Timestamp.ToString("O"));
            sb.Append(" [");
            sb.Append(e.Level);
            sb.Append("] ");
            sb.Append(e.Category);
            sb.Append(": ");
            sb.Append(e.Message);
            if (e.Properties != null && e.Properties.Count > 0)
            {
                sb.Append(" | ");
                foreach (var kv in e.Properties)
                {
                    sb.Append(kv.Key);
                    sb.Append('=');
                    sb.Append(kv.Value?.ToString());
                    sb.Append("; ");
                }
            }
            if (e.Exception != null)
            {
                sb.Append(" => ");
                sb.Append(e.Exception);
            }
            await _writer.WriteAsync(sb.ToString()).ConfigureAwait(false);
        }
    }

    public void Dispose() => _writer.Dispose();
}
