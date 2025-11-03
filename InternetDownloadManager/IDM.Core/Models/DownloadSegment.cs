using IDM.Core.Enums;

namespace IDM.Core.Models;

public class DownloadSegment
{
    public int SegmentId { get; init; }
    public long StartByte { get; init; }
    public long EndByte { get; init; }
    public long DownloadedBytes { get; set; }
    public required string TempFilePath { get; set; }
    public SegmentStatus Status { get; set; }
    public string? ErrorMessage { get; set; }

    public long TotalBytes => EndByte - StartByte + 1;
    public double Progress => TotalBytes > 0 ? (double)DownloadedBytes / TotalBytes * 100 : 0;
}
