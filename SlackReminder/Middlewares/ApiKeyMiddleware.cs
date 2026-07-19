using System.Text.Encodings.Web;
using System.Text.Json;

namespace SlackReminder.Middlewares;

public sealed class ApiKeyMiddleware
{
    #region Fields & Constructor
    private const string ApiKeyHeaderName = "X-API-KEY";
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }
    #endregion

    #region Middleware Invoke Method
    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine($"Method: {context.Request.Method}");

        // 1 Allow Swagger
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        // 2 Allow OPTIONS (CORS preflight)
        if (HttpMethods.Options.Equals(context.Request.Method))
        {
            await _next(context);
            return;
        }

        // 3 Read API key from header
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            await WriteErrorResponse(
                context,
                StatusCodes.Status401Unauthorized,
                "API_KEY_MISSING",
                $"Header '{ApiKeyHeaderName}' is required."
            );
            return;
        }

        // 4 Compare with configured key
        var apiKey = _configuration.GetValue<string>("ApiKey");

        if (string.IsNullOrWhiteSpace(apiKey) || apiKey != extractedApiKey)
        {
            await WriteErrorResponse(
                context,
                StatusCodes.Status401Unauthorized,
                "API_KEY_INVALID",
                "The provided API key is invalid."
            );
            return;
        }

        // 5 Continue pipeline
        await _next(context);
    }
    #endregion

    #region Helper Method to Write Error Response
    private static async Task WriteErrorResponse(
        HttpContext context,
        int statusCode,
        string errorCode,
        string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = statusCode,
            error = errorCode,
            message
        };

        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
    #endregion
}