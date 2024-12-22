using ConstructionManagementSaaS.Models.Requests;
using ConstructionManagementSaaS.Services;
using ConstructionManagementService.Models.Requests;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var response = await _userService.CreateUserAsync(request);
            return CreatedAtAction(nameof(CreateUser), new { id = response.Id }, response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userService.AuthenticateAsync(request);

        if (user == null)
        {
            return Unauthorized(new { message = "Login failed. Please check your email or username and password." });
        }

        // If authentication is successful, return the user (excluding password in response)
        return Ok(new { user });
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _userService.UpdateUserAsync(id, request);

            if (!updated)
                return NotFound(new { Message = "User not found or no changes made." });

            return Ok(new { Message = "User updated successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            // Log the exception if necessary
            return StatusCode(500, new { Message = "An error occurred while updating the user.", Details = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();

            if (users == null || !users.Any())
            {
                return NotFound(new { Message = "No users found." });
            }

            return Ok(users);
        }
        catch (Exception ex)
        {
            // Log the exception if necessary
            return StatusCode(500, new { Message = "An error occurred while retrieving the users.", Details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound(new { Message = "User not found." });

            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            // Log the exception if necessary
            return StatusCode(500, new { Message = "An error occurred while retrieving the user.", Details = ex.Message });
        }
    }

}
