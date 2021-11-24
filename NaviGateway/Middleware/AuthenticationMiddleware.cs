using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NaviGateway.Service;

namespace NaviGateway.Middleware
{
    [ExcludeFromCodeCoverage]
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly UserService _userService;

        public AuthenticationMiddleware(RequestDelegate requestDelegate, UserService userService)
        {
            _requestDelegate = requestDelegate;
            _userService = userService;
        }

        public async Task Invoke(HttpContext context)
        {
            var userToken = context.Request.Headers["X-API-AUTH"].FirstOrDefault();

            if (userToken != null)
            {
                // Authenticate from Authentication Service
                var authenticateResult = await _userService.AuthenticateUser(userToken);

                // If result is success
                if (authenticateResult != null) context.Items["userEmail"] = authenticateResult;
            }
            
            await _requestDelegate(context);
        }
    }
}