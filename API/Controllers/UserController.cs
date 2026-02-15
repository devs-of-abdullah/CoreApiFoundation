
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DTO.User;
using Business.Interfaces;



    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase 
    {
        readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
         _userService = userService;
        }

        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<ActionResult<ReadUserDTO>> GetUserById(int id, [FromServices] IAuthorizationService authorizationService)
        {
            if (id < 1)
                return BadRequest("Invalid user id.");

            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound("User not found.");

        
            var authResult = await authorizationService.AuthorizeAsync(User, id, "StudentOwnerOrAdmin");

         
            if (!authResult.Succeeded)
                return Forbid(); 

            return Ok(user);
        }


        [Authorize(Roles = "Admin")] 
        [HttpPost(Name = "AddUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReadUserDTO>> AddUser(CreateUserDTO newUser)
        {
            if (
               string.IsNullOrEmpty(newUser.Email)
            || string.IsNullOrEmpty(newUser.Password)
            || string.IsNullOrEmpty(newUser.Role)
            )
            {
                return BadRequest("Missed user data.");
            }
            var isExists = _userService.GetByEmailAsync(newUser.Email);
          
            int createdId = await _userService.CreateAsync(newUser);


            if (createdId > 0) return Ok($"User created with id: {createdId}");


            return BadRequest("Error while creating");

        }


    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}", Name = "DeleteUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteStudent(int id)
    {
        return Ok();
       
    }
        


    }

