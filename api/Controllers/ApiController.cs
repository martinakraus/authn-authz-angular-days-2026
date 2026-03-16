using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPIApplication.Controllers;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class CreateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

[Route("api")]
[ApiController]
public class ApiController : ControllerBase
{
    private static List<User> _users = new List<User>
    {
        new User { Id = 1, Name = "Max Mustermann", Email = "max@example.com" },
        new User { Id = 2, Name = "Erika Musterfrau", Email = "erika@example.com" },
        new User { Id = 3, Name = "Hans Schmidt", Email = "hans@example.com" }
    };
    private static int _nextId = 4;
    [HttpGet("users")]
    [Authorize("users:read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetUsers()
    {
        return Ok(_users);
    }

    [HttpGet("users/{id}")]
    [Authorize("users:read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetUserById(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound(new { Message = $"User with ID {id} not found." });
        }
        return Ok(user);
    }

    [HttpPost("users")]
    [Authorize("users:write")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new { Message = "Name and Email are required." });
        }

        var newUser = new User
        {
            Id = _nextId++,
            Name = request.Name,
            Email = request.Email
        };

        _users.Add(newUser);
        return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
    }

    [HttpGet("claims")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Claims()
    {
        return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
    }
}