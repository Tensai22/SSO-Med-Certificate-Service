using MedicalCertificate.Application.CQRS.Commands;
using MedicalCertificate.Application.CQRS.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MedicalCertificate.WebAPI.Security;

namespace MedicalCertificate.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Error);
        }

        return Ok(new { UserId = result.Value });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await mediator.Send(command);

        if (result.IsFailed) {
            return Unauthorized(result.Error);
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var userResult = await mediator.Send(new GetUserByIdQuery(userId.Value));
        if (userResult.IsFailed || userResult.Value is null)
        {
            return NotFound();
        }

        return Ok(new
        {
            userId = userId.Value,
            email = User.FindFirstValue(ClaimTypes.Email),
            role = User.FindFirstValue(ClaimTypes.Role),
            user = userResult.Value
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logged out" });
    }
}
