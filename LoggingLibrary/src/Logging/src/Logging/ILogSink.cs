using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Logging;
public interface ILogSink
{
    /// <summary>
    /// Emit a batch of events to the sink. Implementations should be resilient and non-blocking.
    /// </summary>
    Task EmitBatchAsync(IEnumerable<LogEvent> events, CancellationToken ct);
}
