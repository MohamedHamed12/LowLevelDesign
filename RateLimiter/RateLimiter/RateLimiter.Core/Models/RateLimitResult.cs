namespace RateLimiter.Core.Models;

public class RateLimitResult
{
    public bool IsAllowed { get; set; }
    public int RemainingRequests { get; set; }
    public TimeSpan? RetryAfter { get; set; }
    public string Reason { get; set; } = string.Empty;
    
    public static RateLimitResult Allow(int remaining)
    {
        return new RateLimitResult
        {
            IsAllowed = true,
            RemainingRequests = remaining,
            Reason = "Request allowed"
        };
    }
    
    public static RateLimitResult Deny(TimeSpan? retryAfter = null, string reason = "Rate limit exceeded")
    {
        return new RateLimitResult
        {
            IsAllowed = false,
            RemainingRequests = 0,
            RetryAfter = retryAfter,
            Reason = reason
        };
    }
}
