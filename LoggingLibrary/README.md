# Logging Library

This project is a C# library for logging. It provides a flexible and extensible logging framework with support for different log levels, sinks, and formatting.

## Features

- **Log Levels:** Supports different log levels like Debug, Info, Warning, Error, etc.
- **Sinks:** Includes sinks for console and file logging.
- **Extensible:** Designed to be extensible with custom sinks and formatters.
- **Logger Factory:** Provides a factory for creating logger instances.

## How to Use

1.  Add a reference to the `LoggingLibrary` project.
2.  Create a logger instance using the `LoggerFactory`.
3.  Use the logger to log messages at different levels.

```csharp
var logger = LoggerFactory.CreateLogger();
logger.Info("This is an informational message.");
logger.Error("This is an error message.");
```
