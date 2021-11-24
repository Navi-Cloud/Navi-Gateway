using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NaviGateway.Attribute;
using NaviGateway.Model.Request;
using NaviGateway.Service;

namespace NaviGateway.Controllers
{
    [ApiController]
    [Route("/api/v1/user")]
    public class AuthenticationController: ControllerBase
    {
        private readonly UserService _userService;
        
        public AuthenticationController(UserService userService)
        {
            _userService = userService;
        }
        
        [HttpPost("join")]
        public async Task<IActionResult> RegisterUser(RegisterRequest request)
        {
            await _userService.RegisterUser(request);
            return Ok();
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> LoginUserAsync(LoginRequest loginRequest)
        {
            return Ok(await _userService.LoginUser(loginRequest));
        }
        
        [HttpDelete]
        [AuthenticationNeeded]
        public async Task<IActionResult> RemoveAccountAsync()
        {
            var request = new AccountRemovalRequest
            {
                UserEmail = HttpContext.Items["userEmail"] as string
            };
            await _userService.RemoveUser(request);

            return Ok();
        }
    }
}