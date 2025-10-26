using System.Collections.Concurrent;
using RateLimiter.Core.Interfaces;
using RateLimiter.Core.Models;

namespace RateLimiter.Core.Strategies;

public class FixedWindowLimiter : IRateLimiterStrategy
{
    private readonly ConcurrentDictionary<string, (int Count, DateTime WindowStart)> _windows;
    private readonly SemaphoreSlim _lock;
    private readonly TimeSpan _windowSize;
    private readonly int _maxRequests;
    
    public FixedWindowLimiter(TimeSpan windowSize, int maxRequests)
    {
        _windowSize = windowSize;
        _maxRequests = maxRequests;
        _windows = new ConcurrentDictionary<string, (int, DateTime)>();
        _lock = new SemaphoreSlim(1, 1);
    }
    
    public async Task<RateLimitResult> IsAllowedAsync(string clientId, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var now = DateTime.UtcNow;
            var currentWindow = GetCurrentWindowStart(now);
            
            var window = _windows.AddOrUpdate(
                clientId,
                _ => (1, currentWindow),
                (_, existing) =>
                {
                    if (existing.WindowStart < currentWindow)
                    {
                        return (1, currentWindow);
                    }
                    return (existing.Count + 1, existing.WindowStart);
                });
            
            if (window.Count <= _maxRequests)
            {
                var remaining = _maxRequests - window.Count;
                return RateLimitResult.Allow(remaining);
            }
            
            var nextWindow = window.WindowStart.Add(_windowSize);
            var retryAfter = nextWindow - now;
            return RateLimitResult.Deny(retryAfter);
        }
        finally
        {
            _lock.Release();
        }
    }
    
    public Task ResetAsync(string clientId, CancellationToken cancellationToken = default)
    {
        _windows.TryRemove(clientId, out _);
        return Task.CompletedTask;
    }
    
    public Task<int> GetRemainingRequestsAsync(string clientId, CancellationToken cancellationToken = default)
    {
        if (_windows.TryGetValue(clientId, out var window))
        {
            var now = DateTime.UtcNow;
            var currentWindow = GetCurrentWindowStart(now);
            
            if (window.WindowStart >= currentWindow)
            {
                return Task.FromResult(Math.Max(0, _maxRequests - window.Count));
            }
        }
        
        return Task.FromResult(_maxRequests);
    }
    
    private DateTime GetCurrentWindowStart(DateTime now)
    {
        var ticks = now.Ticks / _windowSize.Ticks;
        return new DateTime(ticks * _windowSize.Ticks, DateTimeKind.Utc);
    }
}
