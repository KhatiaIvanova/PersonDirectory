using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace PersonDirectory.API.Middlewares;
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var error = new { message = ex.Message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(error));
        }
    }
}

public static class ExceptionHandlingExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}