using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using User.Api.Contracts.Requests;
using User.API.Contracts.Requests;
using User.Application.Commands;
using User.Application.Queries;

namespace User.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
    {
        var command = new CreateUserCommand(
            request.Email,
            request.Password,
            request.RoleId,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Address);

        var userId = await _mediator.Send(command);

        return StatusCode(StatusCodes.Status201Created, new { UserId = userId });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = GetAuthenticatedUserId();

        var query = new GetUserByIdQuery(userId);
        var user = await _mediator.Send(query);

        if (user == null) return NotFound();

        return Ok(user);
    }

    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = GetAuthenticatedUserId();

        var command = new UpdateUserProfileCommand(
            userId,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Address);

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("me")]
    [Authorize]
    public async Task<IActionResult> DeleteMyAccount()
    {
        var userId = GetAuthenticatedUserId();

        var command = new DeleteUserCommand(userId);
        await _mediator.Send(command);

        return NoContent();
    }

    private Guid GetAuthenticatedUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            throw new UnauthorizedAccessException("Token inválido o sin ID de usuario.");
        }

        return userId;
    }
}