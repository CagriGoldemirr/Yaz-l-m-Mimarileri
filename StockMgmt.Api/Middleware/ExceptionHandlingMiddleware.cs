using System.Net;
using System.Text.Json;
using StockMgmt.Core;
using StockMgmt.Core.Exceptions;

namespace StockMgmt.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;

        ApiResponse<object> apiResponse;

        switch (exception)
        {
            case NotFoundException notFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                apiResponse = ApiResponse<object>.Fail(notFoundException.Message);
                _logger.LogWarning(exception, "Not Found: {Message}", notFoundException.Message);
                break;

            case ConflictException conflictException:
                response.StatusCode = (int)HttpStatusCode.Conflict;
                apiResponse = ApiResponse<object>.Fail(conflictException.Message);
                _logger.LogWarning(exception, "Conflict: {Message}", conflictException.Message);
                break;

            case ValidationException validationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                var validationMessage = validationException.Errors.Count > 0
                    ? string.Join("; ", validationException.Errors.SelectMany(e => e.Value.Select(v => $"{e.Key}: {v}")))
                    : validationException.Message;
                apiResponse = ApiResponse<object>.Fail(validationMessage, validationException.Errors);
                _logger.LogWarning(exception, "Validation Error: {Message}", validationMessage);
                break;

            case KeyNotFoundException keyNotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                apiResponse = ApiResponse<object>.Fail(keyNotFoundException.Message);
                _logger.LogWarning(exception, "Key Not Found: {Message}", keyNotFoundException.Message);
                break;

            case InvalidOperationException invalidOperationException:
                // Check if it's a conflict scenario (duplicate key, etc.)
                if (invalidOperationException.Message.Contains("already exists") || 
                    invalidOperationException.Message.Contains("already taken"))
                {
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    apiResponse = ApiResponse<object>.Fail(invalidOperationException.Message);
                    _logger.LogWarning(exception, "Conflict: {Message}", invalidOperationException.Message);
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse = ApiResponse<object>.Fail(invalidOperationException.Message);
                    _logger.LogWarning(exception, "Invalid Operation: {Message}", invalidOperationException.Message);
                }
                break;

            case ArgumentException argumentException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                apiResponse = ApiResponse<object>.Fail(argumentException.Message);
                _logger.LogWarning(exception, "Argument Error: {Message}", argumentException.Message);
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse = ApiResponse<object>.Fail("An error occurred while processing your request.");
                _logger.LogError(exception, "Unhandled Exception: {Message}", exception.Message);
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }
}

