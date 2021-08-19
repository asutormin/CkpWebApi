using Microsoft.AspNetCore.Http;
using NLog;
using System;
using System.Net;
using System.Threading.Tasks;
using ILogger = NLog.ILogger;

namespace CkpWebApi.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка: {ex.Message}, TargetSite: {ex.TargetSite}, StackTrace: {ex.StackTrace}");
                await HandleGlobalExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleGlobalExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(
                new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Что-то пошло не так! Ошибка сервера."
                }.ToString());
        }
    }
}
