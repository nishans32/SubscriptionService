using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SubscriptionService.Web.Exceptions;

namespace SubscriptionService.Web.Exceptions
{
    public class ErrorHandlingMiddleware
    {
        public readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var exHandlerPathFeature = context.Features.Get<IExceptionHandlerFeature>();
            var exception = exHandlerPathFeature.Error;

            if (exception is ValidationException)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync(JsonConvert.SerializeObject($"Error : {exception.Message}"));
            }

            await _next.Invoke(context);
        }
    }

    public static class ErrorHandlingMiddlewareExtention
    {
        public static void UseErrorHandling(this IApplicationBuilder builder)
        {
            builder.UseMiddleware(typeof(ErrorHandlingMiddleware));
        }
    }
}
