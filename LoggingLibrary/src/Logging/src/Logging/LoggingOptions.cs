using System;
using System.Collections.Generic;

namespace Logging;
public sealed class LoggingOptions
{
    public int BatchSize { get; set; } = 50;
    public TimeSpan BatchInterval { get; set; } = TimeSpan.FromSeconds(2);
    public int ChannelCapacity { get; set; } = 10000;
    public List<ILogSink> Sinks { get; } = new();
}
