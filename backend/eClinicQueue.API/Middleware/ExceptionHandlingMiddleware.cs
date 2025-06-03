using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Net;

namespace eClinicQueue.API.Middleware;

public class ApiErrorDetails
{
    public ApiErrorDetails()
    {
        Message = string.Empty;
    }

    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string? Detail { get; set; }
    public IEnumerable<string>? ValidationErrors { get; set; }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    var exception = contextFeature.Error;
                    
                    var errorDetails = new ApiErrorDetails();
                    
                    switch (exception)
                    {
                        case ValidationException validationException:
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            errorDetails.StatusCode = context.Response.StatusCode;
                            errorDetails.Message = "Validation error";
                            errorDetails.ValidationErrors = validationException.Errors
                                .Select(e => $"{e.PropertyName}: {e.ErrorMessage}");
                            Log.Warning(exception, "Validation error occurred");
                            break;

                        case UnauthorizedAccessException:
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            errorDetails.StatusCode = context.Response.StatusCode;
                            errorDetails.Message = exception.Message;
                            Log.Warning(exception, "Unauthorized access attempt");
                            break;

                        case SecurityTokenException:
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            errorDetails.StatusCode = context.Response.StatusCode;
                            errorDetails.Message = exception.Message;
                            Log.Warning(exception, "Security token exception");
                            break;

                        case InvalidOperationException:
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            errorDetails.StatusCode = context.Response.StatusCode;
                            errorDetails.Message = exception.Message;
                            Log.Warning(exception, "Invalid operation");
                            break;

                        default:
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            errorDetails.StatusCode = context.Response.StatusCode;
                            errorDetails.Message = "An error occurred while processing your request";
                            
                            #if DEBUG
                            errorDetails.Detail = exception.ToString();
                            #endif
                            
                            Log.Error(exception, "Unhandled exception");
                            break;
                    }

                    await context.Response.WriteAsJsonAsync(errorDetails);
                }
            });
        });
    }
}