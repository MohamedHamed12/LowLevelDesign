using Microsoft.AspNetCore.Mvc;
using RateLimiter.Core.Services;

namespace RateLimiter.API.Controllers;

[ApiController]
[Route("api")]
public class ApiController : ControllerBase
{
    private readonly RateLimiterService _rateLimiterService;
    private readonly ILogger<ApiController> _logger;
    
    public ApiController(RateLimiterService rateLimiterService, ILogger<ApiController> logger)
    {
        _rateLimiterService = rateLimiterService;
        _logger = logger;
    }
    
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new
        {
            message = "Request successful",
            timestamp = DateTime.UtcNow
        });
    }
    
    [HttpGet("status/{clientId}")]
    public async Task<IActionResult> GetRateLimitStatus(string clientId)
    {
        var remaining = await _rateLimiterService.GetRemainingRequestsAsync(clientId);
        
        return Ok(new
        {
            clientId,
            remainingRequests = remaining,
            timestamp = DateTime.UtcNow
        });
    }
    
    [HttpPost("reset/{clientId}")]
    public async Task<IActionResult> ResetRateLimit(string clientId)
    {
        await _rateLimiterService.ResetClientLimitAsync(clientId);
        
        _logger.LogInformation("Rate limit reset for client {ClientId}", clientId);
        
        return Ok(new
        {
            message = $"Rate limit reset for client {clientId}",
            timestamp = DateTime.UtcNow
        });
    }
}
