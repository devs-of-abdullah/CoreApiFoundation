using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using DTO.Auth;


    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            var tokens =  _authService.Login(request);
            if (tokens == null)
                return Unauthorized("Invalid credentials");

            return Ok(tokens);
        }

        [HttpPost("refresh")]
        [EnableRateLimiting("AuthLimiter")]
        public IActionResult Refresh([FromBody] RefreshRequestDTO request)
        {
            var tokens = _authService.RefreshToken(request);
            if (tokens == null)
                return Unauthorized("Invalid refresh request");

            return Ok(tokens);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDTO request)
        {
            await _authService.Logout(request);
            return Ok("Logged out successfully");
        }
    }
