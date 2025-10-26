using RateLimiter.API.Extensions;
using RateLimiter.API.Middleware;
using RateLimiter.Core.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add rate limiting with custom configuration
builder.Services.AddRateLimiting(config =>
{
    config.Algorithm = RateLimitAlgorithm.TokenBucket;
    config.Capacity = 100;
    config.RefillRate = 10.0; // 10 tokens per second
    config.WindowSize = TimeSpan.FromMinutes(1);
    config.MaxRequests = 100;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add rate limiting middleware
app.UseMiddleware<RateLimitingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
