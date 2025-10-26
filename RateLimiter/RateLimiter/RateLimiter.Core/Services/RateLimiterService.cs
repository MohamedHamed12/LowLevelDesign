using System.Collections.Concurrent;
using RateLimiter.Core.Enums;
using RateLimiter.Core.Interfaces;
using RateLimiter.Core.Models;
using RateLimiter.Core.Strategies;

namespace RateLimiter.Core.Services;

public class RateLimiterService
{
    private readonly ConcurrentDictionary<string, IRateLimiterStrategy> _strategies;
    private readonly IRateLimiterStrategy _defaultStrategy;
    
    public RateLimiterService(RateLimiterConfig defaultConfig)
    {
        defaultConfig.Validate();
        _strategies = new ConcurrentDictionary<string, IRateLimiterStrategy>();
        _defaultStrategy = CreateStrategy(defaultConfig);
    }
    
    public async Task<RateLimitResult> CheckRateLimitAsync(
        string clientId, 
        string resource = "default", 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(clientId))
            throw new ArgumentException("Client ID cannot be null or empty", nameof(clientId));
        
        var strategy = _strategies.GetValueOrDefault(resource, _defaultStrategy);
        return await strategy.IsAllowedAsync(clientId, cancellationToken);
    }
    
    public void RegisterStrategy(string resource, RateLimiterConfig config)
    {
        config.Validate();
        var strategy = CreateStrategy(config);
        _strategies[resource] = strategy;
    }
    
    public async Task ResetClientLimitAsync(
        string clientId, 
        string resource = "default", 
        CancellationToken cancellationToken = default)
    {
        var strategy = _strategies.GetValueOrDefault(resource, _defaultStrategy);
        await strategy.ResetAsync(clientId, cancellationToken);
    }
    
    public async Task<int> GetRemainingRequestsAsync(
        string clientId, 
        string resource = "default", 
        CancellationToken cancellationToken = default)
    {
        var strategy = _strategies.GetValueOrDefault(resource, _defaultStrategy);
        return await strategy.GetRemainingRequestsAsync(clientId, cancellationToken);
    }
    
    private static IRateLimiterStrategy CreateStrategy(RateLimiterConfig config)
    {
        return config.Algorithm switch
        {
            RateLimitAlgorithm.TokenBucket => new TokenBucketLimiter(config.Capacity, config.RefillRate),
            RateLimitAlgorithm.SlidingWindow => new SlidingWindowLimiter(config.WindowSize, config.MaxRequests),
            RateLimitAlgorithm.FixedWindow => new FixedWindowLimiter(config.WindowSize, config.MaxRequests),
            _ => throw new ArgumentException($"Unsupported algorithm: {config.Algorithm}")
        };
    }
}
