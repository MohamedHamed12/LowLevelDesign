namespace IDM.Infrastructure.Configuration;

public class DownloadConfiguration
{
    public const string SectionName = "Download";

    public int MaxConcurrentDownloads { get; set; } = 3;
    public int MaxSegmentsPerDownload { get; set; } = 8;
    public int BufferSize { get; set; } = 8192;
    public int RetryDelaySeconds { get; set; } = 5;
    public int MaxRetries { get; set; } = 3;
    public required string TempDirectory { get; set; } = Path.Combine(Path.GetTempPath(), "IDM", "Temp");
    public required string MetadataDirectory { get; set; } = Path.Combine(Path.GetTempPath(), "IDM", "Metadata");
    public int ProgressUpdateIntervalMs { get; set; } = 500;
    public long MinFileSizeForSegmentation { get; set; } = 1024 * 1024; // 1 MB
}
