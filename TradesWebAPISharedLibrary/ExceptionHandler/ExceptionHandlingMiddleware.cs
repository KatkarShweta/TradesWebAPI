using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace TradesWebAPISharedLibrary.ExceptionHandler
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate _next)
        {
            try
            {
                await _next(context);
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "BadRequest exception occurred.");
                await HandleBadRequestAsync(context, ex);
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex, "NotFound exception occurred.");
                await HandleNotFoundAsync(context, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled runtime exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        //handle 400 bad request error
        private static async Task HandleBadRequestAsync(HttpContext context, BadRequestException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = exception.Message
            };

            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));

        }

        //handle 404 not found exception
        private static async Task HandleNotFoundAsync(HttpContext context, NotFoundException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = exception.Message
            };

            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }

        //handle internal server error
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Runtime Exception : Internal Server Error"
            };

            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }

    }
}
