using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Text.Json;

namespace Discoteque.Business.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled Exception");

            var problemDetails = ex switch
            {
                InvalidOperationException => new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Invalid Operation",
                    Detail = ex.Message,
                    Instance = context.Request.Path
                },
                DbUpdateConcurrencyException => new ProblemDetails
                {
                    Status = (int)HttpStatusCode.Conflict,
                    Title = "Concurrency Conflict",
                    Detail = "The data has been modified. Please refresh and try again.",
                    Instance = context.Request.Path
                },
                DbUpdateException => new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Database Update Error",
                    Detail = "Unable to update the database. Please check your data and try again.",
                    Instance = context.Request.Path
                },
                UnauthorizedAccessException => new ProblemDetails
                {
                    Status = (int)HttpStatusCode.Unauthorized,
                    Title = "Unauthorized Access",
                    Detail = "You are not authorized to perform this action.",
                    Instance = context.Request.Path
                },
                _ => new ProblemDetails
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "An unexpected error occurred",
                    Detail = _env.IsDevelopment() ? ex.Message : "An internal server error occurred.",
                    Instance = context.Request.Path
                }
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;


            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
        }
    }
}