using projectTest.Repositories.Interfaces;

namespace projectTest.Middleware;

public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;

    public ApiKeyAuthenticationMiddleware(RequestDelegate next, ILogger<ApiKeyAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IApiKeyRepository apiKeyRepository)
    {
        // Skip API Key check if JWT token is present
        if (context.User.Identity?.IsAuthenticated == true)
        {
            await _next(context);
            return;
        }

        // Check for API Key in header
        if (context.Request.Headers.TryGetValue("X-API-KEY", out var apiKeyHeader))
        {
            var apiKey = apiKeyHeader.ToString();
            if (!string.IsNullOrEmpty(apiKey))
            {
                var isValid = await apiKeyRepository.IsValidAsync(apiKey);
                if (isValid)
                {
                    // Create a claims identity for API Key authentication
                    var claims = new[]
                    {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "ApiKey"),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "ApiKey")
                    };
                    var identity = new System.Security.Claims.ClaimsIdentity(claims, "ApiKey");
                    context.User = new System.Security.Claims.ClaimsPrincipal(identity);

                    _logger.LogInformation("API Key authentication successful");
                    await _next(context);
                    return;
                }
                else
                {
                    _logger.LogWarning("Invalid API Key attempted: {ApiKey}", apiKey);
                }
            }
        }

        // Continue to next middleware (JWT authentication will be checked later)
        await _next(context);
    }
}
