using RateLimiter.Core.Models;

namespace RateLimiter.Core.Interfaces;

public interface IRateLimiterStrategy
{
    Task<RateLimitResult> IsAllowedAsync(string clientId, CancellationToken cancellationToken = default);
    Task ResetAsync(string clientId, CancellationToken cancellationToken = default);
    Task<int> GetRemainingRequestsAsync(string clientId, CancellationToken cancellationToken = default);
}
