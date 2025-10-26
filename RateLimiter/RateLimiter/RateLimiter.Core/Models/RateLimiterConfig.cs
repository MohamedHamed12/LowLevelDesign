using RateLimiter.Core.Enums;

namespace RateLimiter.Core.Models;

public class RateLimiterConfig
{
    public int MaxRequests { get; set; } = 100;
    public TimeSpan WindowSize { get; set; } = TimeSpan.FromMinutes(1);
    public RateLimitAlgorithm Algorithm { get; set; } = RateLimitAlgorithm.TokenBucket;
    
    // Token Bucket specific
    public int Capacity { get; set; } = 100;
    public double RefillRate { get; set; } = 10.0; // tokens per second
    
    public void Validate()
    {
        if (MaxRequests <= 0)
            throw new ArgumentException("MaxRequests must be greater than 0", nameof(MaxRequests));
        
        if (WindowSize <= TimeSpan.Zero)
            throw new ArgumentException("WindowSize must be greater than 0", nameof(WindowSize));
        
        if (Algorithm == RateLimitAlgorithm.TokenBucket)
        {
            if (Capacity <= 0)
                throw new ArgumentException("Capacity must be greater than 0", nameof(Capacity));
            
            if (RefillRate <= 0)
                throw new ArgumentException("RefillRate must be greater than 0", nameof(RefillRate));
        }
    }
}
