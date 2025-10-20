using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Logging;
public sealed class Logger : ILogger
{
    private readonly string _category;
    private readonly LoggerFactory _factory;

    internal Logger(string category, LoggerFactory factory)
    {
        _category = category ?? throw new ArgumentNullException(nameof(category));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public void Log(LogLevel level, string message, Exception? ex = null, IDictionary<string, object?>? props = null)
    {
        if (!_factory.IsEnabled(level)) return;

        var evt = new LogEvent
        {
            Timestamp = DateTimeOffset.UtcNow,
            Level = level,
            Category = _category,
            Message = message,
            Exception = ex,
            Properties = props is null ? new Dictionary<string, object?>() : new Dictionary<string, object?>(props)
        };

        // try to write to channel; drop if full (or could block depending on design).
        _factory.Enqueue(evt);
    }
}
