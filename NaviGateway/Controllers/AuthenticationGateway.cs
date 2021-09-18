using System.Collections.Generic;
using System.Threading.Tasks;
using Io.Github.NaviCloud.Shared;
using Io.Github.NaviCloud.Shared.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NaviGateway.Attribute;
using NaviGateway.Factory;
using Newtonsoft.Json;

namespace NaviGateway.Controllers
{
    [ApiController]
    [Route("/api/v1/user")]
    public class AuthenticationGateway: SuperController
    {
        private readonly Authentication.AuthenticationClient _client;

        public AuthenticationGateway(ClientFactory clientFactory)
        {
            _client = clientFactory.AuthenticationClient;
        }
        
        [HttpPost("join")]
        public async Task<IActionResult> RegisterUser(RegisterRequest request)
        {
            var result = await _client.RegisterUserAsync(request);
            var handledCase = new Dictionary<ResultType, LazyExecution>
            {
                [ResultType.Duplicate] = () => Conflict(result.Message),
                [ResultType.Success] = () => Ok()
            };

            return HandleCase(handledCase, result);
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> LoginUserAsync(LoginRequest loginRequest)
        {
            var result = await _client.LoginUserAsync(loginRequest);

            var handledCase = new Dictionary<ResultType, LazyExecution>
            {
                [ResultType.Forbidden] = () => new ObjectResult(result.Message) { StatusCode = StatusCodes.Status403Forbidden },
                [ResultType.Success] = () =>
                {
                    var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.Object);
                    return Ok(jsonObject);
                }
            };

            return HandleCase(handledCase, result);
        }
        
        [HttpDelete]
        [AuthenticationNeeded]
        public async Task<IActionResult> RemoveAccountAsync()
        {
            var request = new AccountRemovalRequest
            {
                UserEmail = HttpContext.Items["userEmail"] as string
            };
            var result = await _client.RemoveUserAsync(request);

            var handledCase = new Dictionary<ResultType, LazyExecution>
            {
                [ResultType.Success] = () => Ok("Bye-Bye!")
            };

            return HandleCase(handledCase, result);
        }
    }
}