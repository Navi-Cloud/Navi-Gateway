using System.Threading.Tasks;
// using LoggerLibrary;
using Microsoft.AspNetCore.Http;
using NaviGateway.Service;

namespace NaviGateway.Middleware
{
    public class RequestLoggerMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly LoggerService _loggerService;
        
        public RequestLoggerMiddleware(RequestDelegate requestDelegate, LoggerService loggerService)
        {
            _requestDelegate = requestDelegate;
            _loggerService = loggerService;
        }
        
        public async Task Invoke(HttpContext context)
        {
            // Request Logging
            await _loggerService.LogInfoMessageAsync($"Requested: {context.Request.Path}, with ID {context.TraceIdentifier}");
                
            // Do next
            await _requestDelegate(context);
                
            // Response Logging
            await _loggerService.LogInfoMessageAsync($"Request ID {context.TraceIdentifier} Responded: {context.Response.StatusCode}");
        }
    }
}