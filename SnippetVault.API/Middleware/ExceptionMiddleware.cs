using System.Text.Json;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ExceptionMiddleware> _logger; // Inject the logger

    public ExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _env = env;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        int statusCode;
        string message;

        if (ex is KeyNotFoundException)
        {
            _logger.LogWarning("[WARNING] Resource not found: {Message}", ex.Message);
            statusCode = StatusCodes.Status404NotFound;
            message = ex.Message;
        }
        else if (ex is UnauthorizedAccessException)
        {
            _logger.LogWarning("[WARNING] Unauthorized access attempt: {Message}", ex.Message);
            statusCode = StatusCodes.Status403Forbidden;
            message = ex.Message;
        }
        else if (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("[WARNING] Conflict - duplicate resource: {Message}", ex.Message);
            statusCode = StatusCodes.Status409Conflict;
            message = ex.Message;
        }
        else
        {
            // Unexpected crash — log as Error with the full exception details
            _logger.LogError(ex, "[ERROR] Unhandled exception: {Message}", ex.Message);
            statusCode = StatusCodes.Status500InternalServerError;
            message = _env.IsDevelopment()
                ? $"An unexpected error occurred: {ex.Message}"
                : "An unexpected error occurred. Please try again later.";
        }
           
        context.Response.StatusCode = statusCode;
       
        var response = JsonSerializer.Serialize(new { message });
        await context.Response.WriteAsync(response);
    }
}