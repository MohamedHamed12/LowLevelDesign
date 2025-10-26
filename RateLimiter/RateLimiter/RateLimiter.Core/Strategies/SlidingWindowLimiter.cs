using System.Collections.Concurrent;
using RateLimiter.Core.Interfaces;
using RateLimiter.Core.Models;

namespace RateLimiter.Core.Strategies;

public class SlidingWindowLimiter : IRateLimiterStrategy
{
    private readonly ConcurrentDictionary<string, SlidingWindow> _windows;
    private readonly ReaderWriterLockSlim _lock;
    private readonly TimeSpan _windowSize;
    private readonly int _maxRequests;
    
    public SlidingWindowLimiter(TimeSpan windowSize, int maxRequests)
    {
        _windowSize = windowSize;
        _maxRequests = maxRequests;
        _windows = new ConcurrentDictionary<string, SlidingWindow>();
        _lock = new ReaderWriterLockSlim();
    }
    
    public Task<RateLimitResult> IsAllowedAsync(string clientId, CancellationToken cancellationToken = default)
    {
        var window = _windows.GetOrAdd(clientId, _ => new SlidingWindow(_windowSize, _maxRequests));
        
        _lock.EnterWriteLock();
        try
        {
            if (window.TryAddRequest())
            {
                var remaining = window.GetRemainingRequests();
                return Task.FromResult(RateLimitResult.Allow(remaining));
            }
            
            var retryAfter = window.GetRetryAfter();
            return Task.FromResult(RateLimitResult.Deny(retryAfter));
        }
        finally
        {
            _lock.ExitWriteLock();
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
            _lock.EnterReadLock();
            try
            {
                return Task.FromResult(window.GetRemainingRequests());
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        
        return Task.FromResult(_maxRequests);
    }
    
    public void Dispose()
    {
        _lock?.Dispose();
    }
}
