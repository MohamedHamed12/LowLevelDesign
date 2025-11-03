# Internet Download Manager (IDM) - .NET 9

A high-performance, multi-threaded download manager built with .NET 9, featuring segmented downloads, pause/resume functionality, and automatic retry mechanisms.

## Features

- **Multi-threaded Downloads**: Split files into multiple segments for faster downloads
- **Pause/Resume**: Pause and resume downloads at any time
- **Auto-retry**: Automatic retry on network failures
- **Download Queue**: Queue multiple downloads with priority support
- **Progress Tracking**: Real-time speed and ETA calculation
- **Persistent State**: Resume downloads after application restart
- **Crash Recovery**: Automatically recover from crashes

## Architecture

### Core Components

- **DownloadManager**: Main orchestrator for all download operations
- **DownloadEngine**: Handles download execution and segmentation logic
- **SegmentDownloader**: Downloads individual file segments with retry logic
- **DownloadQueue**: Thread-safe queue for managing pending downloads
- **StorageManager**: Handles file I/O and metadata persistence
- **ProgressTracker**: Calculates download speed and ETA

## Project Structure

```
InternetDownloadManager/
├── IDM.Core/                  # Core domain logic and interfaces
│   ├── Interfaces/            # Service interfaces
│   ├── Models/                # Domain models
│   ├── Enums/                 # Enumerations
│   ├── Events/                # Event arguments
│   └── Exceptions/            # Custom exceptions
├── IDM.Infrastructure/        # Implementation of core interfaces
│   ├── Services/              # Service implementations
│   ├── Storage/               # File storage management
│   ├── Http/                  # HTTP client wrappers
│   └── Configuration/         # Configuration models
├── IDM.Console/               # Console application
└── IDM.Tests/                 # Unit tests
```

## Getting Started

### Prerequisites

- .NET 9 SDK
- Windows, Linux, or macOS

### Installation

1. Run the setup script:
```bash
chmod +x setup.sh
./setup.sh
```

2. Build the solution:
```bash
dotnet build
```

3. Run the console application:
```bash
cd IDM.Console
dotnet run
```

### Configuration

Edit `appsettings.json` to customize download settings:

```json
{
  "Download": {
    "MaxConcurrentDownloads": 3,
    "MaxSegmentsPerDownload": 8,
    "BufferSize": 8192,
    "RetryDelaySeconds": 5,
    "MaxRetries": 3,
    "TempDirectory": "./Downloads/Temp",
    "MetadataDirectory": "./Downloads/Metadata",
    "MinFileSizeForSegmentation": 1048576
  }
}
```

## Usage

### Console Application

The interactive console provides the following options:

1. **Add Download**: Add a new download by providing URL and destination path
2. **View Downloads**: View all downloads with status and progress
3. **Pause Download**: Pause an active download
4. **Resume Download**: Resume a paused download
5. **Cancel Download**: Cancel and remove a download

### Programmatic Usage

```csharp
// Setup dependency injection
var services = new ServiceCollection();
services.AddDownloadManager(configuration);
var serviceProvider = services.BuildServiceProvider();

// Get download manager
var downloadManager = serviceProvider.GetRequiredService<IDownloadManager>();

// Add a download
var task = await downloadManager.AddDownloadAsync(
    "https://example.com/file.zip", 
    "/path/to/destination.zip");

// Subscribe to events
task.ProgressChanged += (sender, e) =>
{
    Console.WriteLine($"Progress: {e.Progress:F2}% - Speed: {e.Speed} bytes/s");
};

task.StatusChanged += (sender, e) =>
{
    Console.WriteLine($"Status changed: {e.OldStatus} -> {e.NewStatus}");
};

// Pause download
await downloadManager.PauseDownloadAsync(task.Id);

// Resume download
await downloadManager.ResumeDownloadAsync(task.Id);

// Cancel download
await downloadManager.CancelDownloadAsync(task.Id);
```

## Design Patterns Used

- **Repository Pattern**: StorageManager abstracts data persistence
- **Factory Pattern**: IHttpClientFactory for HTTP client management
- **Observer Pattern**: Event-based progress and status notifications
- **Strategy Pattern**: Different download strategies based on file size and server support
- **Dependency Injection**: Loose coupling between components
- **SOLID Principles**: Single responsibility, interface segregation

## Testing

Run unit tests:
```bash
dotnet test
```

## Performance Considerations

- **Segmented Downloads**: Files larger than 1MB are automatically split into multiple segments
- **Connection Pooling**: Reuses HTTP connections for better performance
- **Async I/O**: Non-blocking file operations
- **Buffered Writes**: Configurable buffer size for optimal I/O
- **Memory Efficiency**: Streams data rather than loading entire files in memory

## Error Handling

- Automatic retry with exponential backoff
- Graceful degradation to single-stream download if segmentation fails
- Comprehensive logging at all levels
- Persistent state for crash recovery

## Future Enhancements

- [ ] FTP/SFTP protocol support
- [ ] Browser integration
- [ ] Bandwidth throttling
- [ ] Scheduled downloads
- [ ] Download categories
- [ ] Checksum verification (MD5, SHA256)
- [ ] Web UI with ASP.NET Core
- [ ] Download acceleration algorithms
- [ ] Torrent support

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

MIT License - feel free to use this project for learning or commercial purposes.

## Author

Built with .NET 9 and best practices in mind.
