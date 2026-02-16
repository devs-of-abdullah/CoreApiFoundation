using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using DTO.Auth;
using Business.Services;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [EnableRateLimiting("AuthLimiter")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        var tokens = await _authService.Login(request);

        if (tokens == null)
            return Unauthorized(new { message = "Invalid credentials" });

        return Ok(tokens);
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("AuthLimiter")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDTO request)
    {
        var tokens = await _authService.RefreshToken(request);

        if (tokens == null)
            return Unauthorized(new { message = "Invalid refresh request" });

        return Ok(tokens);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDTO request)
    {
        await _authService.Logout(request);
        return Ok(new { message = "Logged out successfully" });
    }
}
