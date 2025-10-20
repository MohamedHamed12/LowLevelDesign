using System;
using Logging;

Console.WriteLine("Sample Logging App");

var options = new LoggingOptions
{
    BatchSize = 20,
    BatchInterval = TimeSpan.FromSeconds(1),
    ChannelCapacity = 1000
};

options.Sinks.Add(new ConsoleSink());
options.Sinks.Add(new FileSink("logs", "app.log", maxBytes: 1024 * 1024, maxFiles: 3));

using var factory = new LoggerFactory(options);
factory.SetMinimumLevel(LogLevel.Debug);

var logger = factory.CreateLogger("MyApp.Category");

logger.Info("Application started");
for (int i = 0; i < 100; i++)
{
    logger.Debug("Processing item {i}", null, new System.Collections.Generic.Dictionary<string, object?> { ["i"] = i });
}

logger.Error("Simulated error", new Exception("boom"), new System.Collections.Generic.Dictionary<string, object?> { ["user"] = "mohamed" });

Console.WriteLine("Waiting a bit for logs to flush...");
await Task.Delay(2500);

Console.WriteLine("Exiting sample");
