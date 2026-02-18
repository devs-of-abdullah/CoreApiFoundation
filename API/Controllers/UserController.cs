using Business.Interfaces;
using DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;


[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthorizationService _authorizationService;

    public UsersController(IUserService userService, IAuthorizationService authorizationService)
    {
        _userService = userService;
        _authorizationService = authorizationService;
    }


    [HttpGet("{id:int}", Name = "GetUserById")]
    
    [EnableRateLimiting("AuthLimiter")]

    public async Task<ActionResult<ReadUserDTO>> GetById(int id, [FromServices] IAuthorizationService authorizationService)
    {
        if (id <= 0)
            return BadRequest("Invalid user ID.");

        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return NotFound("User not found.");

        var authResult = await authorizationService.AuthorizeAsync(User, id, "UserOwnerOrAdmin");

        if (!authResult.Succeeded) return Forbid();

        return Ok(user);
    }

    
    [HttpPost(Name = "CreateUser")]
    [EnableRateLimiting("AuthLimiter")]

    public async Task<ActionResult<ReadUserDTO>> Create([FromBody] CreateUserDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingUser = await _userService.GetByEmailAsync(dto.Email);
        if (existingUser != null)
            return Conflict("User email already exists.");

        var createdId = await _userService.CreateAsync(dto);
        if (createdId <= 0)
            return BadRequest("Error while creating user.");

        var createdUser = await _userService.GetByIdAsync(createdId);

        return CreatedAtRoute("GetUserById", new { id = createdId }, createdUser);
    }

  
    [Authorize]
    [HttpPut("change-password", Name = "ChangeUserPassword")]
    [EnableRateLimiting("AuthLimiter")]

    public async Task<IActionResult> ChangePassword([FromBody] UpdateUserPasswordDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        await _userService.UpdatePasswordAsync(userId, dto);

        return NoContent();
    }

    [Authorize]
    [HttpDelete("self", Name = "SelfDelete")]
    public async Task<IActionResult> SelfDelete([FromBody] SoftUserDeleteDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        await _userService.SoftDeleteAsync(userId, dto);

        return NoContent();
    }


    [Authorize(Roles = "admin")]
    [HttpDelete("{id:int}", Name = "OutDelete")]
    public async Task<IActionResult> OutDelete(int id, [FromBody] SoftUserDeleteDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _userService.SoftDeleteAsync(id, dto);

        return NoContent();
    }
}
