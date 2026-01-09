using System.Collections.Concurrent;

namespace projectTest.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private static readonly ConcurrentDictionary<string, RateLimitInfo> _rateLimitStore = new();
    private const int MaxRequestsPerMinute = 100;

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        
        var now = DateTime.UtcNow;
        var key = $"ratelimit:{clientId}";

        if (_rateLimitStore.TryGetValue(key, out var rateLimitInfo))
        {
            // Reset if a minute has passed
            if (now - rateLimitInfo.WindowStart > TimeSpan.FromMinutes(1))
            {
                rateLimitInfo.Count = 0;
                rateLimitInfo.WindowStart = now;
            }

            if (rateLimitInfo.Count >= MaxRequestsPerMinute)
            {
                _logger.LogWarning("Rate limit exceeded for client: {ClientId}", clientId);
                context.Response.StatusCode = 429;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\":\"TooManyRequests\",\"message\":\"Rate limit exceeded. Maximum 100 requests per minute.\"}");
                return;
            }

            rateLimitInfo.Count++;
        }
        else
        {
            _rateLimitStore.TryAdd(key, new RateLimitInfo
            {
                Count = 1,
                WindowStart = now
            });
        }

        await _next(context);
    }

    private class RateLimitInfo
    {
        public int Count { get; set; }
        public DateTime WindowStart { get; set; }
    }
}
