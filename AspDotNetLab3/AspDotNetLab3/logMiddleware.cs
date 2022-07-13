using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
namespace AspDotNetLab3
{
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _filePath;
        public LogMiddleware(RequestDelegate next, string filePath)
        {
            _next = next;
            _filePath = filePath;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            await File.AppendAllTextAsync(_filePath,
            $"{DateTime.UtcNow} " +
            $"{context.Request.Path} " +
            $"{context.Request.HttpContext.Connection.RemoteIpAddress} " +
            $"{context.Request.Protocol} " +
            $"{context.Request.Method}\n");
            if (_next != null)
                await _next.Invoke(context);
        }
    }
    public static class LogMiddlewareExtension
    {
        public static IApplicationBuilder UseLog
        (this IApplicationBuilder builder, string filePath)
        {
            return builder.UseMiddleware<LogMiddleware>(filePath);
        }
    }
}
