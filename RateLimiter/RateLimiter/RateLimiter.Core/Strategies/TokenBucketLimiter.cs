using System.Collections.Concurrent;
using RateLimiter.Core.Interfaces;
using RateLimiter.Core.Models;

namespace RateLimiter.Core.Strategies;

public class TokenBucketLimiter : IRateLimiterStrategy
{
    private readonly ConcurrentDictionary<string, TokenBucket> _buckets;
    private readonly SemaphoreSlim _lock;
    private readonly int _capacity;
    private readonly double _refillRate;
    
    public TokenBucketLimiter(int capacity, double refillRate)
    {
        _capacity = capacity;
        _refillRate = refillRate;
        _buckets = new ConcurrentDictionary<string, TokenBucket>();
        _lock = new SemaphoreSlim(1, 1);
    }
    
    public async Task<RateLimitResult> IsAllowedAsync(string clientId, CancellationToken cancellationToken = default)
    {
        var bucket = _buckets.GetOrAdd(clientId, _ => new TokenBucket(_capacity, _refillRate));
        
        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (bucket.TryConsume())
            {
                return RateLimitResult.Allow((int)bucket.Tokens);
            }
            
            var retryAfter = TimeSpan.FromSeconds(1.0 / _refillRate);
            return RateLimitResult.Deny(retryAfter);
        }
        finally
        {
            _lock.Release();
        }
    }
    
    public Task ResetAsync(string clientId, CancellationToken cancellationToken = default)
    {
        _buckets.TryRemove(clientId, out _);
        return Task.CompletedTask;
    }
    
    public Task<int> GetRemainingRequestsAsync(string clientId, CancellationToken cancellationToken = default)
    {
        if (_buckets.TryGetValue(clientId, out var bucket))
        {
            bucket.Refill();
            return Task.FromResult((int)bucket.Tokens);
        }
        
        return Task.FromResult(_capacity);
    }
}
