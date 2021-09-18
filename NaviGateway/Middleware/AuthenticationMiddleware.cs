using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Io.Github.NaviCloud.Shared;
using Io.Github.NaviCloud.Shared.Authentication;
using Microsoft.AspNetCore.Http;
using NaviGateway.Factory;

namespace NaviGateway.Middleware
{
    [ExcludeFromCodeCoverage]
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly Authentication.AuthenticationClient _authenticationClient;

        public AuthenticationMiddleware(RequestDelegate requestDelegate, ClientFactory clientFactory)
        {
            _requestDelegate = requestDelegate;
            _authenticationClient = clientFactory.AuthenticationClient;
        }

        public async Task Invoke(HttpContext context)
        {
            var userToken = context.Request.Headers["X-API-AUTH"].FirstOrDefault();

            if (userToken != null)
            {
                // Authenticate from Authentication Service
                var result = _authenticationClient.AuthenticateUser(new AuthenticationRequest
                    { UserAccessToken = userToken });
            
                // If result is success
                if (result?.ResultType == ResultType.Success) context.Items["userEmail"] = result.Object;
            }
            
            await _requestDelegate(context);
        }
    }
}