namespace RateLimiter.Core.Models;

public class TokenBucket
{
    public double Tokens { get; set; }
    public DateTime LastRefillTime { get; set; }
    public int Capacity { get; set; }
    public double RefillRate { get; set; }
    
    public TokenBucket(int capacity, double refillRate)
    {
        Capacity = capacity;
        RefillRate = refillRate;
        Tokens = capacity;
        LastRefillTime = DateTime.UtcNow;
    }
    
    public void Refill()
    {
        var now = DateTime.UtcNow;
        var timePassed = (now - LastRefillTime).TotalSeconds;
        var tokensToAdd = timePassed * RefillRate;
        
        Tokens = Math.Min(Capacity, Tokens + tokensToAdd);
        LastRefillTime = now;
    }
    
    public bool TryConsume(int tokens = 1)
    {
        Refill();
        
        if (Tokens >= tokens)
        {
            Tokens -= tokens;
            return true;
        }
        
        return false;
    }
}
