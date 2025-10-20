using System;
using System.Collections.Generic;

namespace Logging;
public static class LoggerExtensions
{
    public static void Trace(this ILogger logger, string msg, Exception? ex = null, IDictionary<string, object?>? props = null)
        => logger.Log(LogLevel.Trace, msg, ex, props);

    public static void Debug(this ILogger logger, string msg, Exception? ex = null, IDictionary<string, object?>? props = null)
        => logger.Log(LogLevel.Debug, msg, ex, props);

    public static void Info(this ILogger logger, string msg, Exception? ex = null, IDictionary<string, object?>? props = null)
        => logger.Log(LogLevel.Info, msg, ex, props);

    public static void Warn(this ILogger logger, string msg, Exception? ex = null, IDictionary<string, object?>? props = null)
        => logger.Log(LogLevel.Warn, msg, ex, props);

    public static void Error(this ILogger logger, string msg, Exception? ex = null, IDictionary<string, object?>? props = null)
        => logger.Log(LogLevel.Error, msg, ex, props);

    public static void Fatal(this ILogger logger, string msg, Exception? ex = null, IDictionary<string, object?>? props = null)
        => logger.Log(LogLevel.Fatal, msg, ex, props);
}
