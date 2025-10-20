using System;
using System.Collections.Generic;

namespace Logging;
public sealed class LogEvent
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    public LogLevel Level { get; init; }
    public string Category { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public IReadOnlyDictionary<string, object?> Properties { get; init; } = new Dictionary<string, object?>();
    public Exception? Exception { get; init; }

    public override string ToString()
        => $"{Timestamp:O} [{Level}] {Category}: {Message}" + (Exception is null ? "" : $" -> {Exception}");
}
