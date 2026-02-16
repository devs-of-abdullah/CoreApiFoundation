using Business.Interfaces;
using DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReadUserDTO>> GetById(int id)
    {
        if (id <= 0)
            return BadRequest("Invalid user id.");

        var user = await _userService.GetByIdAsync(id);

        if (user == null)
            return NotFound("User not found.");

        var authResult = await _authorizationService
            .AuthorizeAsync(User, id, "StudentOwnerOrAdmin");

        if (!authResult.Succeeded)
            return Forbid();

        return Ok(user);
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        return CreatedAtRoute("GetUserById",
            new { id = createdId },
            createdUser);
    }


    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] UpdateUserPasswordDTO dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim);

        await _userService.UpdatePasswordAsync(userId, dto);

        return NoContent();
    }




    [Authorize]
    [HttpDelete("{id:int}")]
   
    public async Task<IActionResult> Delete(SoftUserDeleteDTO dto)
    {

        var userIdClaim = User.FindFirst("id")?.Value;

        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim);

        await _userService.SoftDeleteAsync(userId, dto);

        return NoContent();
    }

}
