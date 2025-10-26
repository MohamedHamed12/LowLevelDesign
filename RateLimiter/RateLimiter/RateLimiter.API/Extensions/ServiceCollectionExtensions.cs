using RateLimiter.Core.Enums;
using RateLimiter.Core.Models;
using RateLimiter.Core.Services;

namespace RateLimiter.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRateLimiting(
        this IServiceCollection services, 
        Action<RateLimiterConfig>? configureOptions = null)
    {
        var config = new RateLimiterConfig
        {
            Algorithm = RateLimitAlgorithm.TokenBucket,
            Capacity = 100,
            RefillRate = 10.0,
            MaxRequests = 100,
            WindowSize = TimeSpan.FromMinutes(1)
        };
        
        configureOptions?.Invoke(config);
        
        services.AddSingleton(new RateLimiterService(config));
        
        return services;
    }
}
