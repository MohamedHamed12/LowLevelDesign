using Microsoft.AspNetCore.Mvc;
using RateLimiter.Core.Services;

namespace ECommerceApp.API.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly RateLimiterService _rateLimiter;
    private readonly ILogger<CartController> _logger;

    public CartController(RateLimiterService rateLimiter, ILogger<CartController> logger)
    {
        _rateLimiter = rateLimiter;
        _logger = logger;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddItem([FromQuery] string clientId)
    {
        var result = await _rateLimiter.CheckRateLimitAsync(clientId, "cart-add");

        if (!result.IsAllowed)
        {
            return StatusCode(429, new
            {
                message = "Rate limit exceeded. Try again later.",
                retryAfter = result.RetryAfter?.TotalSeconds
            });
        }

        _logger.LogInformation("Item added to cart for {ClientId}", clientId);

        return Ok(new
        {
            message = "Item added successfully",
            remaining = result.RemainingRequests,
            timestamp = DateTime.UtcNow
        });
    }
}
