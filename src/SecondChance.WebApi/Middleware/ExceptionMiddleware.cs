using SecondChance.Application.DTOs.Common;
using SecondChance.Application.Exceptions;
using System.Text.Json;

namespace SecondChance.WebApi.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            (int statusCode, string message) = ex switch
            {
                BadRequestException => (400, ex.Message),
                InvalidOperationException => (409, ex.Message),
                ArgumentNullException => (400, ex.Message),
                ArgumentException => (400, ex.Message),
                UnauthorizedAccessException => (401, ex.Message),
                KeyNotFoundException => (404, ex.Message),
                _ => (500, "An unexpected error occurred. Please try again later.")
            };

            context.Response.StatusCode = statusCode;
            var response = ApiResponse<object>.Fail(message);
            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
