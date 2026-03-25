using Auth.Application.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(Application.Commands.Login.LoginRequest request)
    {
        var response = await _mediator.Send(request);

        if (!response.Success)
            return Unauthorized(new { error = response.Error });

        return Ok(new { token = response.Token });
    }
}