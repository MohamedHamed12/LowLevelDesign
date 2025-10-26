using System.Net;
using RateLimiter.Core.Services;

namespace RateLimiter.API.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RateLimiterService _rateLimiterService;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    
    public RateLimitingMiddleware(
        RequestDelegate next, 
        RateLimiterService rateLimiterService,
        ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _rateLimiterService = rateLimiterService;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);
        var resource = GetResourceIdentifier(context);
        
        var result = await _rateLimiterService.CheckRateLimitAsync(clientId, resource);
        
        context.Response.Headers.Append("X-RateLimit-Remaining", result.RemainingRequests.ToString());
        
        if (!result.IsAllowed)
        {
            _logger.LogWarning(
                "Rate limit exceeded for client {ClientId} on resource {Resource}", 
                clientId, 
                resource);
            
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            
            if (result.RetryAfter.HasValue)
            {
                context.Response.Headers.Append(
                    "Retry-After", 
                    ((int)result.RetryAfter.Value.TotalSeconds).ToString());
            }
            
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Rate limit exceeded",
                message = result.Reason,
                retryAfter = result.RetryAfter?.TotalSeconds
            });
            
            return;
        }
        
        await _next(context);
    }
    
    private static string GetClientIdentifier(HttpContext context)
    {
        // Try to get API key from header
        if (context.Request.Headers.TryGetValue("X-API-Key", out var apiKey))
        {
            return $"apikey:{apiKey}";
        }
        
        // Try to get user from authentication
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            return $"user:{context.User.Identity.Name}";
        }
        
        // Fall back to IP address
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return $"ip:{ipAddress}";
    }
    
    private static string GetResourceIdentifier(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        return endpoint?.DisplayName ?? context.Request.Path;
    }
}
