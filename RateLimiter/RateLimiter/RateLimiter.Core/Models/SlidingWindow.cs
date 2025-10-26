namespace RateLimiter.Core.Models;

public class SlidingWindow
{
    private readonly Queue<DateTime> _requestTimestamps;
    private readonly TimeSpan _windowSize;
    private readonly int _maxRequests;
    
    public SlidingWindow(TimeSpan windowSize, int maxRequests)
    {
        _windowSize = windowSize;
        _maxRequests = maxRequests;
        _requestTimestamps = new Queue<DateTime>();
    }
    
    public bool TryAddRequest()
    {
        var now = DateTime.UtcNow;
        CleanupOldEntries(now);
        
        if (_requestTimestamps.Count < _maxRequests)
        {
            _requestTimestamps.Enqueue(now);
            return true;
        }
        
        return false;
    }
    
    public int GetRemainingRequests()
    {
        CleanupOldEntries(DateTime.UtcNow);
        return Math.Max(0, _maxRequests - _requestTimestamps.Count);
    }
    
    public TimeSpan? GetRetryAfter()
    {
        if (_requestTimestamps.Count == 0)
            return null;
        
        var oldestRequest = _requestTimestamps.Peek();
        var windowEnd = oldestRequest.Add(_windowSize);
        var retryAfter = windowEnd - DateTime.UtcNow;
        
        return retryAfter > TimeSpan.Zero ? retryAfter : null;
    }
    
    private void CleanupOldEntries(DateTime now)
    {
        var cutoffTime = now.Subtract(_windowSize);
        
        while (_requestTimestamps.Count > 0 && _requestTimestamps.Peek() < cutoffTime)
        {
            _requestTimestamps.Dequeue();
        }
    }
}
