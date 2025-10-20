using System;
using System.Collections.Generic;

namespace Logging;
public interface ILogger
{
    void Log(LogLevel level, string message, Exception? ex = null, IDictionary<string, object?>? properties = null);
}
