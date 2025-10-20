using System;
using System.IO;
using System.Threading.Tasks;

namespace Logging.Internal;
internal sealed class RollingFileWriter
{
    private readonly string _directory;
    private readonly string _fileName;
    private readonly long _maxBytes;
    private readonly int _maxFiles;
    private FileStream? _currentStream;
    private string _currentPath = string.Empty;
    private long _currentSize = 0;

    public RollingFileWriter(string directory, string fileName, long maxBytes, int maxFiles)
    {
        _directory = directory ?? throw new ArgumentNullException(nameof(directory));
        _fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        _maxBytes = maxBytes;
        _maxFiles = Math.Max(1, maxFiles);
        Directory.CreateDirectory(_directory);
        OpenNew();
    }

    private void OpenNew()
    {
        _currentPath = Path.Combine(_directory, _fileName);
        _currentStream?.Dispose();
        _currentStream = new FileStream(_currentPath, FileMode.Append, FileAccess.Write, FileShare.Read, 4096, FileOptions.Asynchronous);
        _currentSize = _currentStream.Length;
    }

    private void RotateIfNeeded()
    {
        if (_currentSize < _maxBytes) return;
        _currentStream?.Dispose();
        // shift older files
        for (int i = _maxFiles - 1; i >= 1; i--)
        {
            var src = Path.Combine(_directory, $"{_fileName}.{i - 1}");
            var dst = Path.Combine(_directory, $"{_fileName}.{i}");
            if (File.Exists(dst)) File.Delete(dst);
            if (File.Exists(src)) File.Move(src, dst);
        }
        var first = Path.Combine(_directory, $"{_fileName}.0");
        if (File.Exists(_currentPath)) File.Move(_currentPath, first);
        OpenNew();
    }

    public async Task WriteAsync(string text)
    {
        if (_currentStream == null) OpenNew();
        var bytes = System.Text.Encoding.UTF8.GetBytes(text + Environment.NewLine);
        await _currentStream!.WriteAsync(bytes.AsMemory(0, bytes.Length));
        await _currentStream.FlushAsync();
        _currentSize += bytes.Length;
        RotateIfNeeded();
    }

    public void Dispose()
    {
        _currentStream?.Dispose();
        _currentStream = null;
    }
}
