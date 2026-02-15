// Copyright (C) TBC Bank. All Rights Reserved.

using System.Net;
using System.Text.Json;
using Discounts.Application.Exceptions;
using Discounts.Application.Exceptions.ResponceFormat;

namespace Discounts.API.Infrastructure.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TraceId: {TraceId}, Message: {Message}", context.TraceIdentifier, ex.Message);
                await HandleExceptionAsync(context, ex).ConfigureAwait(true);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = new ApiResponse<string>("An error occurred.");

            switch (exception)
            {
                case ValidationException validationEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Validation Failed";
                    response.Errors = validationEx.Errors.SelectMany(x => x.Value).ToList();
                    break;

                case NotFoundException notFoundEx:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = notFoundEx.Message;
                    break;

                case BadRequestException badRequestEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = badRequestEx.Message;
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = "You are not authorized to perform this action.";
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = _env.IsDevelopment()
                        ? exception.Message
                        : "Internal Server Error. Please contact support.";
                    break;
            }

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json).ConfigureAwait(true);
        }
    }
}
