using RateLimiter.Core.Enums;
using RateLimiter.Core.Models;
using RateLimiter.Core.Services;

namespace ECommerceApp.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRateLimitingService(this IServiceCollection services)
    {
        var config = new RateLimiterConfig
        {
            Algorithm = RateLimitAlgorithm.FixedWindow,
            MaxRequests = 5,
            WindowSize = TimeSpan.FromSeconds(10)
        };

        services.AddSingleton(new RateLimiterService(config));
        return services;
    }
}
