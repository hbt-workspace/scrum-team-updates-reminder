namespace SlackReminder.Middlewares;

public static class ApiKeyMiddlewareExtensions
{
    #region Extension Method
    public static IApplicationBuilder UseApiKeyAuthentication(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ApiKeyMiddleware>();
    }
    #endregion
}