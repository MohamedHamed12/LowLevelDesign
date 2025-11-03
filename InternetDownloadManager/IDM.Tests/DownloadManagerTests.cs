using IDM.Core.Interfaces;
using IDM.Core.Models;
using IDM.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace IDM.Tests;

public class DownloadManagerTests
{
    private readonly Mock<IDownloadQueue> _mockQueue;
    private readonly Mock<IDownloadEngine> _mockEngine;
    private readonly Mock<IStorageManager> _mockStorage;
    private readonly Mock<ILogger<DownloadManager>> _mockLogger;
    private readonly DownloadManager _downloadManager;

    public DownloadManagerTests()
    {
        _mockQueue = new Mock<IDownloadQueue>();
        _mockEngine = new Mock<IDownloadEngine>();
        _mockStorage = new Mock<IStorageManager>();
        _mockLogger = new Mock<ILogger<DownloadManager>>();
        
        _downloadManager = new DownloadManager(
            _mockQueue.Object,
            _mockEngine.Object,
            _mockStorage.Object,
            _mockLogger.Object,
            3);
    }

    [Fact]
    public async Task AddDownloadAsync_ShouldCreateDownloadTask()
    {
        // Arrange
        var url = "https://example.com/file.zip";
        var destination = "/downloads/file.zip";

        // Act
        var task = await _downloadManager.AddDownloadAsync(url, destination);

        // Assert
        task.Should().NotBeNull();
        task.Url.Should().Be(url);
        task.Destination.Should().Be(destination);
        _mockQueue.Verify(q => q.Enqueue(It.IsAny<DownloadTask>()), Times.Once);
        _mockStorage.Verify(s => s.SaveMetadataAsync(It.IsAny<DownloadTask>(), default), Times.Once);
    }

    [Fact]
    public async Task GetDownloadAsync_ShouldReturnDownloadTask()
    {
        // Arrange
        var task = await _downloadManager.AddDownloadAsync("https://example.com/file.zip", "/downloads/file.zip");

        // Act
        var retrieved = await _downloadManager.GetDownloadAsync(task.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved.Should().Be(task);
    }
}
