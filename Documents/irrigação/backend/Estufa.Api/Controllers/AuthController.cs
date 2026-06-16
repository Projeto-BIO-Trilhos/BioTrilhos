using Estufa.Api.Models.Auth;
using Estufa.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estufa.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            try
            {
                var user = await _auth.RegisterAsync(req.Nome, req.Email, req.Password);
                return Ok(new { user.Id, user.Nome, user.Email });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var token = await _auth.LoginAsync(req.Email, req.Password);
            if (token == null) return Unauthorized();
            return Ok(new AuthResponse { Token = token, ExpiresIn = "configured" });
        }
    }
}
